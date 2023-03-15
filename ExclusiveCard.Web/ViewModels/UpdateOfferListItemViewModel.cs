using ExclusiveCard.Services.Models.DTOs.Admin;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class UpdateOfferListItemViewModel
    {
        public int OfferListItemId { get; set; }
        public string CountryCode { get; set; }
        public List<OfferSummary> OfferListItems { get; set; }

        public UpdateOfferListItemViewModel()
        {
            OfferListItems = new List<OfferSummary>();
        }
    }
}
