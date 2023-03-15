using System;

namespace ExclusiveCard.Website.Models.Api
{
    public class DealsSearchRequest
    {
        public Guid AppId { get; set; }
        public int? DealId { get; set; }
        public string Category { get; set; }
        public string Keyword { get; set; }
        public Location Location { get; set; }
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public DealsSearchRequest()
        {
            Location = new Location();
        }
    }
}
