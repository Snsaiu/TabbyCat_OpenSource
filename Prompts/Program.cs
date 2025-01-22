internal class Program
{
    private static void Main()
    {
        string inputFilePath = "./roleEnglish.txt";
        string outputFilePath = "./roleEnglishWithComma.txt";

        // 读取文件内容
        string[] lines = File.ReadAllLines(inputFilePath);

        // 在每一行单词后面加上 ","
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i] + ",";
        }

        // 将修改后的内容写回到新文件
        File.WriteAllLines(outputFilePath, lines);

        Console.WriteLine("文件处理完成！");
    }
}