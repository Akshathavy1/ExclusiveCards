using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class TransactionLogList
    {
        public List<TransactionViewModel> Transactions { get; set; }
        public PagingViewModel PagingViewModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public TransactionLogSortField TransactionLogSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }

        public TransactionLogList(TransactionLogRequest request)
        {
            TransactionLogSortField = request.TransactionSortField;
            SortDirection = request.SortDirection;
            SortIcon = request.SortIcon;
            Transactions = new List<TransactionViewModel>();
            PagingViewModel = new PagingViewModel();
        }
    }
}
