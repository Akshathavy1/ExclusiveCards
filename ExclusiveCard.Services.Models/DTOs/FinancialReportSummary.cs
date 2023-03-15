using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class FinancialReportSummary
    {
        public Int64 TotalRecord { get; set; }
        public Int64 RowNumber { get; set; }
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
