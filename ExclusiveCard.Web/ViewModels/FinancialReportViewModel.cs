namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FinancialReportViewModel
    {
        public string Description { get; set; }
        public decimal ExclusiveCommission { get; set; }
        public decimal BeneficiaryCommission { get; set; }
        public string Beneficiary { get; set; }
        public decimal TalkSportCommission { get; set; }
        public int ClickCount { get; set; }
        public int CustomerCount { get; set; }
        public decimal CashbackAmount { get; set; }
    }
}
