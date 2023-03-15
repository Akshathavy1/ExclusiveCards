using System.Collections.Generic;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MerchantBranchListViewModel
    {
        //public int MerchantId { get; set; }
        public List<DTOs.MerchantBranch> ListofBranches { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int CurrentPageNumber { get; set; }
        public MerchantBranchListViewModel()
        {
            ListofBranches = new List<DTOs.MerchantBranch>();
            PagingModel = new PagingViewModel();
        }
    }
}
