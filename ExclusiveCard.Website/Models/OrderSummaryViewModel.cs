using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class OrderSummaryViewModel
    {
        public int MembershipPlanId { get; set; }
        public decimal TotalPrice { get; set; }
        public string PayPalButtonAppAndCardRef { get; set; }
        public string PayPalButtonAppRef { get; set; }
        public string PayPalLink { get; set; }
        public List<OrderDetailsViewModel> OrderDetails { get; set; }

        public OrderSummaryViewModel()
        {
            OrderDetails = new List<OrderDetailsViewModel>();
        }
    }

    public class OrderDetailsViewModel
    {
        public string OrderName { get; set; }
        public bool IsSelected { get; set; }
        public decimal CardPrice { get; set; }
    }
}
