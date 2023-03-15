using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class TransactionsViewModel
    {
        public List<TransactionViewModel> Transactions { get; set; }
        public List<CustomerWithdrawViewModel> CustomerWithdrawals { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public PartnerTransactionSortField TransactionSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }

        public TransactionsViewModel(TransactionRequest request)
        {
            TransactionSortField = request.TransactionSortField;
            SortDirection = request.SortDirection;
            SortIcon = request.SortIcon;
            Transactions = new List<TransactionViewModel>();
            CustomerWithdrawals = new List<CustomerWithdrawViewModel>();
            PagingModel = new PagingViewModel();
        }
    }
}
