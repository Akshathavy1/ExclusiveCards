using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class OfferHubList
    {
        public List<OfferHubViewModel> ListOfOffer { get; set; }
        public PagingViewModel PagingViewModel { get; set; }
        public int? CurrentPageNumber { get; set; }

        public OfferHubList()
        {
            ListOfOffer = new List<OfferHubViewModel>();
            PagingViewModel = new PagingViewModel();
        }
    }
}
