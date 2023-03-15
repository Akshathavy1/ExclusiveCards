namespace ExclusiveCard.WebAdmin.Models.Api
{
    public class GetDealRequest
    {
        public string AppId { get; set; }
        public int? DealId { get; set; }
        public object Category { get; set; }
        public object Keyword { get; set; }
        public object Location { get; set; }
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
