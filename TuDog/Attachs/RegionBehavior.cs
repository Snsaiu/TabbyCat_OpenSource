using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using TuDog.Bases.Regions;
using TuDog.Bootstrap;
using TuDog.ViewLocators;

namespace TuDog.Attachs;

public class RegionBehavior:AvaloniaObject
{
    static RegionBehavior()
    {
        RegionProperty.Changed.AddClassHandler<AvaloniaObject>(HandleRegionChanged);
    }

    private static void HandleRegionChanged(AvaloniaObject arg1, AvaloniaPropertyChangedEventArgs arg2)
    {
        if(arg1 is not ContentControl contentControl)
            return;
        var regionContainer = TuDogApplication.ServiceProvider?.GetRequiredService<RegionContainerBase>();
        if (regionContainer is null)
            throw new NullReferenceException("RegionContainer is null");
        if(arg2.NewValue is not null and var v)
        {
            if(v.ToString() is not  null and var vs)
                regionContainer.TryAdd(vs,contentControl);
        }
       
    }


    // 添加一个string类型的名为Region的附加属性
   public static readonly AttachedProperty<string> RegionProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject,string>("Region",typeof(RegionBehavior),string.Empty,false);
   
   // 获取Region的值
   public static string GetRegion(AvaloniaObject obj)
   {
       return obj.GetValue(RegionProperty);
   }
   
    // 设置Region的值
    public static void SetRegion(AvaloniaObject obj, string value)
    {
        obj.SetValue(RegionProperty, value);
    }
   
}