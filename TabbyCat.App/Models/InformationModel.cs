using Newtonsoft.Json;

using TabbyCat.Shared.Enums;

namespace TabbyCat.App.Models
{
    public class InformationModel
    {
        public SendType SendType { get; set; }

        public string? Text { get; set; }

        public string? FolderPath { get; set; }

        public IEnumerable<string>? Files { get; set; }

        [JsonIgnore]
        public string? Summary => SendType switch
        {
            SendType.Text => Text?[..Math.Min(Text.Length, 20)],
            SendType.Folder => FolderPath?[..Math.Min(FolderPath.Length, 20)],
            SendType.File => string.Join(",", Files?.Select(Path.GetFileName) ?? throw new ArgumentNullException())[
                ..Math.Min(string.Join(",", Files.Select((Path.GetFileName))).Length, 20)],
            _ => throw new NotImplementedException()
        };


    }
}