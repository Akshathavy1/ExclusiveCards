using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class LocalOfferList
    {
        public List<LocalOfferViewModel> ListOfOffer { get; set; }
        public PagingViewModel PagingViewModel { get; set; }
        public int? CurrentPageNumber { get; set; }

        public LocalOfferList()
        {
            ListOfOffer = new List<LocalOfferViewModel>();
            PagingViewModel = new PagingViewModel();
        }
    }
}
