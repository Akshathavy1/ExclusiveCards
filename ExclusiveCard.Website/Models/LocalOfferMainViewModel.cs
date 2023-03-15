using System.Collections.Generic;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class LocalOfferMainViewModel
    {
        public List<LocalOfferViewModel> LocalOffer { get; set; }
        public List<LocalOfferViewModel> BestCashbackOffers { get; set; }
        public List<LocalOfferViewModel> HomepageOffers { get; set; }
        public List<dto.Category> Categories { get; set; }
        public List<dto.WhiteLabelSettings> WhiteLabel { get; set; }
        public List<LocalOffer> whiteLableList { get; set; }
        public string WhiteLableName { get; set; }
        public LocalOfferMainViewModel()
        {
            LocalOffer = new List<LocalOfferViewModel>();
            BestCashbackOffers = new List<LocalOfferViewModel>();
            HomepageOffers = new List<LocalOfferViewModel>();
            Categories = new List<dto.Category>();
            WhiteLabel = new List<dto.WhiteLabelSettings>();
            whiteLableList = new List<LocalOffer>();
        }
    }
    /// <summary>
    /// Ignore the name, this is just an id/name pair
    /// </summary>
    public class LocalOffer
    {
        public int id;
        public string Name;
    }
}
