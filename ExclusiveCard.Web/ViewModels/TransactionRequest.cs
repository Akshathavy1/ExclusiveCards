using ExclusiveCard.Enums;

namespace ExclusiveCard.WebAdmin.ViewModels
{

    public class TransactionRequest
    {
        private PartnerTransactionSortField _transactionSortField;

        public PartnerTransactionSortField TransactionSortField
        {
            get { return _transactionSortField; }
            set { _transactionSortField = (PartnerTransactionSortField)value; }
        }

        private string _sortDirection = "asc";
        public string SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set { if (!(string.IsNullOrEmpty(value))) { _sortDirection = value; } }
        }

        private string _sorticon = "fa fa-sort-alpha-asc";

        public string SortIcon
        {
            get
            {
                return _sorticon;
            }
            set { if (!(string.IsNullOrEmpty(value))) { _sorticon = value; } }
        }
    }
}
