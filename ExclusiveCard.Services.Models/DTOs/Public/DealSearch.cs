namespace ExclusiveCard.Services.Models.DTOs.Public
{
    public class DealSearch
    {
        public int AppId { get; set; }
        public int? DealId { get; set; }
        public string Category { get; set; }
        public string Keyword { get; set; }
        public Location Location { get; set; }
        public string UserToken { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? DealTypeId { get; set; }
    }
}
