using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExclusiveCard.Services.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class SearchFinancialReportViewModel
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        [DisplayName("Withdrawal Status")]
        public int StatusId { get; set; }
        public List<SelectListItem> Status { get; set; }
       
        public FinancialReport FinancialReport { get; set; }

        public SearchFinancialReportViewModel()
        {
            Status = new List<SelectListItem>();
            var firstDayOfThisMonth = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day - 1));
            StartDate = firstDayOfThisMonth.AddMonths(-1);
            EndDate = firstDayOfThisMonth.AddDays(-1);
        }
    }
}