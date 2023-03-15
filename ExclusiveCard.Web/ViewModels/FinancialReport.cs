using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FinancialReport
    {
        public List<FinancialReportViewModel> FinancialReportViewModel { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public WithdrawalSortField WithdrawalSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }

        public FinancialReport()
        {
            FinancialReportViewModel = new List<FinancialReportViewModel>();
            PagingModel = new PagingViewModel();
        }
    }
}
