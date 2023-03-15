using System.IO;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class ExternalFile
    {
        public string FileName { get; set; }
        public FileStream FileContent { get; set; }
        public MemoryStream FileMemoryContent { get; set; }
    }
}
