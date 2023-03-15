using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Public = ExclusiveCard.Services.Models.DTOs.Public;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class AccountSummaryViewModel
    {
        public string UserName { get; set; }
        public int CustomerId { get; set; }

        [DisplayName("Select Card:")]
        public string CardId { get; set; }
        public List<SelectListItem> ListofCards { get; set; }

       
        public decimal AvailableCashbackAmount { get; set; }
        public AccountChartViewModel CashBackChart { get; set; }
        public AccountChartViewModel GiftedChart { get; set; }
        public List<Public.TransactionSummary> TransactionSummaries { get; set; }
        public PartnerRewards PartnerReward { get; set; }
        public string Country { get; set; }

        public AccountSummaryViewModel()
        {
            ListofCards = new List<SelectListItem>();
            //ListofHistory = new List<SelectListItem>();
            CashBackChart = new AccountChartViewModel();
            GiftedChart = new AccountChartViewModel();
            TransactionSummaries = new List<Public.TransactionSummary>();
            PartnerReward = new PartnerRewards();
        }
    }
}
