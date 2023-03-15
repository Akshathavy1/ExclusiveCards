using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CustomerDetailsMembershipCardsViewModel
    {
        public int Id { get; set; }
        [DisplayName("Username:")]
        public string Username { get; set; }
        [DisplayName("Membership Plan:")]
        public string PlanName { get; set; }

        [DisplayName("Card Number:")]
        public string CardNumber { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Issued:")]
        public DateTime? DateIssued { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Expiry Date:")]
        public DateTime? ExpiryDate { get; set; }

        [DisplayName("Status:")]
        public string Status { get; set; }

        [DisplayName("Registration Code:")]
        public string RegistrationCode { get; set; }

        [DisplayName("Physical Card Requested ")]
        public bool CardRequest { get; set; }

        [DisplayName("Physical Card Status:")]
        public int CardStatus { get; set; }

        [DisplayName("Physical Card Status:")]
        public string PhysicalCardStatus { get; set; }

        public DateTime CardRequestedDate { get; set; }

        public List<SelectListItem> CardStatusList { get; set; }

        public CustomerDetailsMembershipCardsViewModel()
        {
          
           CardStatusList = new List<SelectListItem>();
        }
    }
}
