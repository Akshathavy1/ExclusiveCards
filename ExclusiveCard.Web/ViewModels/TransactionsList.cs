using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class TransactionsList
    {
        public List<PartnerTransactionsViewModel> Transactions { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public TransactionSortField TransactionSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }

        public TransactionsList(PartnerTransactionListRequest request)
        {
            TransactionSortField = request.TransactionSortField;
            SortDirection = request.SortDirection;
            SortIcon = request.SortIcon;
            Transactions = new List<PartnerTransactionsViewModel>();
            PagingModel = new PagingViewModel();
        }
    }
}
