using ExclusiveCard.Enums;
using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class WithdrawalsList
    {
        public List<WithdrawalViewModel> Withdrawals { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int? CurrentPageNumber { get; set; }
        public WithdrawalSortField WithdrawalSortField { get; set; }
        public string SortDirection { get; set; }
        public string SortIcon { get; set; }

        public WithdrawalsList(RewardWithdrawalListRequest request)
        {
            WithdrawalSortField = request.WithdrawalSortField;
            SortDirection = request.SortDirection;
            SortIcon = request.SortIcon;
            Withdrawals = new List<WithdrawalViewModel>();
            PagingModel = new PagingViewModel();
        }
    }
}
