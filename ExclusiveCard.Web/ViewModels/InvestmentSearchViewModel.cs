using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class InvestmentSearchViewModel
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        [DisplayName("Reward Partner")]
        public int PartnerId { get; set; }
        public List<SelectListItem> Partners { get; set; }
        public TransactionsViewModel Transactions { get; set; }

        public InvestmentSearchViewModel()
        {
            Partners = new List<SelectListItem>();
            var firstDayOfThisMonth = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day - 1));
            StartDate = firstDayOfThisMonth.AddMonths(-1);
            EndDate = firstDayOfThisMonth.AddDays(-1);
        }
    }
}
