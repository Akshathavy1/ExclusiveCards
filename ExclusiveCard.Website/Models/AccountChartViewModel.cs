namespace ExclusiveCard.Website.Models
{
    public class AccountChartViewModel
    {
        public decimal PendingValue { get; set; }
        public int PendingDetails { get; set; }
        public int PendingPercentage { get; set; }

        public decimal ConfirmedValue { get; set; }
        public int ConfirmedDetails { get; set; }
        public int ConfirmedPercentage { get; set; }

        public decimal ReceivedValue { get; set; }
        public int ReceivedDetails { get; set; }
        public int ReceivedPercentage { get; set; }
    }
}
