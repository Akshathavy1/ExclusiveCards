using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CustomerSearchViewModel
    {
        [DisplayName("Username:")]
        public string Username { get; set; }
        [DisplayName("Forename:")]
        public string Forename { get; set; }
        [DisplayName("Surname:")]
        public string Surname { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Birth:")]
        public DateTime? Dob { get; set; }
        [DisplayName("Postcode:")]
        public string Postcode { get; set; }
        [DisplayName("Card Number:")]
        public string CardNumber { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Issued Card:")]
        public DateTime? CardDateIssued { get; set; }
        [DisplayName("Card Status:")]
        public int? CardStatus { get; set; }

        [DisplayName("Registration Code:")]
        public string RegistrationCode { get; set; }
        public List<SelectListItem> CustomStatusList { get; set; }
        public List<DTOs.CustomerSummary> ListofCustomerSummary { get; set; }
        public PagingViewModel PagingModel { get; set; }
        public int PageNumber { get; set; }

        public CustomerSearchViewModel()
        {
            ListofCustomerSummary = new List<DTOs.CustomerSummary>();
            CustomStatusList = new List<SelectListItem>();
            PagingModel = new PagingViewModel();
        }
    }
}

