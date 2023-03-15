namespace ExclusiveCard.Data.Models
{
    public class WithdrawalRequestModel
    {
        public bool RequestExists { get; set; }
        public int CustomerId { get; set; }
        public int? BankDetailId { get; set; }
        public int? PartnerRewardId { get; set; }
        public decimal? AvailableFund { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
    }
}
