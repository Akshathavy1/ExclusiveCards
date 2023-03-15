using ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class PagedBranchContactViewModel
    {
        public List<MerchantBranch> MerchantBranches { get; set; }

        public PagingViewModel PagingView { get; set; }

        public int? CurrentPageNumber { get; set; }

        public PagedBranchContactViewModel()
        {
            MerchantBranches = new List<MerchantBranch>();
            PagingView = new PagingViewModel();
        }
    }
}
