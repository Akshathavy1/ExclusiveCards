using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class AddEditOfferListItemModel
    {
        public int OfferListId { get; set; }

        public string OfferIds { get; set; }

        public int? DisplayOrder { get; set; }

        public string Countrycode { get; set; }

        public int? MerchantId { get; set; }

        public int? OfferStatus { get; set; }

        public int? AffiliateId { get; set; }

        public string Keyword { get; set; }
        
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int? OfferType { get; set; }

        public int offerPage { get; set; }

        public bool AddTolist { get; set; }

        public int? SelectedOrder { get; set; }

        public int ItemsSelected { get; set; }
    }
}
