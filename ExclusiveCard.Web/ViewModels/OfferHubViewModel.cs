using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferHubViewModel
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Action { get; set; }
    }
}
