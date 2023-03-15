namespace ExclusiveCard.Services.Models.DTOs.Public
{
    public class DealSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool Online { get; set; }
        public string ThumbnailPath { get; set; }
    }
}
