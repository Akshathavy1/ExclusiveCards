using ExclusiveCard.Enums;
using System.Collections.Generic;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MerchantsList
    {
        public List<DTOs.Merchant> ListofMerchants { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public MerchantSortField MerchantSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }
        public MerchantsList(MerchantListRequest request)
        {
            MerchantSortField = request.MerchantSortField;
            SortDirection = request.SortDirection;
            SortIcon = request.SortIcon;
            ListofMerchants = new List<DTOs.Merchant>();
            PagingModel = new PagingViewModel();
        }
    }
}
