
using ExclusiveCard.Enums;

namespace ExclusiveCard.WebAdmin.ViewModels
{

    public class OfferListRequest
    {
        private OfferSortField _sortField = OfferSortField.MerchantName;

        public OfferSortField SortField
        {
            get { return _sortField; }
            set { _sortField = (OfferSortField) value; }
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
