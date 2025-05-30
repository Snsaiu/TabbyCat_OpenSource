using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;

namespace YouYan.Rabbit.Extensions;

public class AutoStartHelper
{
    public static void WindowsSetAutoStart(bool enable)
    {
        var runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        using (var key = Registry.CurrentUser.OpenSubKey(runKey, true))
        {
            if (enable)
            {
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName!;
                key.SetValue("Rabbit", $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue("Rabbit", false);
            }
        }
    }

    public static void MacOSSetAutoStart(
        string plistPath,
        string appName,
        string exePath,
        bool runAtLoad)
    {
        // 读取或创建基础 plist 文档
        XDocument doc;
        if (File.Exists(plistPath))
            doc = XDocument.Load(plistPath);
        else
            doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("plist", new XAttribute("version", "1.0"),
                    new XElement("dict")
                )
            );

        var dict = doc.Descendants("dict").FirstOrDefault();
        if (dict == null)
            throw new InvalidOperationException("无效的 plist 文件，未找到 <dict> 元素。");

        // 工具方法：添加或替换 key-value 对
        void SetKeyValue(string key, XElement value)
        {
            var existingKey = dict.Elements("key").FirstOrDefault(k => k.Value == key);
            if (existingKey != null)
            {
                var existingValue = existingKey.ElementsAfterSelf().FirstOrDefault();
                if (existingValue != null)
                    existingValue.ReplaceWith(value);
                else
                    existingKey.AddAfterSelf(value);
            }
            else
            {
                dict.Add(new XElement("key", key));
                dict.Add(value);
            }
        }

        // 添加 Label（如果缺失）
        SetKeyValue("Label", new XElement("string", appName));

        // 添加 ProgramArguments（如果缺失）
        SetKeyValue("ProgramArguments", new XElement("array", new XElement("string", exePath)));

        // 设置 RunAtLoad（可切换）
        SetKeyValue("RunAtLoad", new XElement(runAtLoad ? "true" : "false"));
        
        SetKeyValue("KeepAlive",new XElement("false"));

        // 保存修改
        Directory.CreateDirectory(Path.GetDirectoryName(plistPath)!);
        doc.Save(plistPath);
    }
}