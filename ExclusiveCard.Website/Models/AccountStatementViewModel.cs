using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Public = ExclusiveCard.Services.Models.DTOs.Public;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Website.Models
{
    public class AccountStatementViewModel
    {
        public string UserName { get; set; }
        public int CustomerId { get; set; }

        [DisplayName("Select Card:")]
        public string MembershipCardId { get; set; }
        public List<SelectListItem> ListofCards { get; set; }

        [DisplayName("Cashback Available")]
        public decimal AvailableCashbackAmount { get; set; }
        [DisplayName("Cashback Earned")]
        public decimal CashbackEarned { get; set; }
        [DisplayName("Total Donated")]
        public decimal TotalDonated { get; set; }
        [DisplayName("Cashback Paid")]
        public decimal CashbackPaid { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [ValidDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        [DisplayName("Date From:")]
        public DateTime? ValidFrom { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [ValidDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        [DisplayName("Date To:")]
        public DateTime? ValidTo { get; set; }

        public AccountChartViewModel CashBackChart { get; set; }
        public AccountChartViewModel GiftedChart { get; set; }
        public dto.PagedResult<Public.TransactionSummary> TransactionSummaries { get; set; }
        public PagingViewModel Paging { get; set; }

        public int CurrentPage { get; set; }
        public string AccountStatus { get; set; }

        public AccountStatementViewModel()
        {
            ListofCards = new List<SelectListItem>();
            CashBackChart = new AccountChartViewModel();
            GiftedChart = new AccountChartViewModel();
            TransactionSummaries = new dto.PagedResult<Public.TransactionSummary>();
            Paging = new PagingViewModel();
        }
    }
}
