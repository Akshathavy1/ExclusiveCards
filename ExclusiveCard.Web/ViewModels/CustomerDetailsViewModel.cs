using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CustomerDetailsViewModel
    {
        [DisplayName("Username:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(256)]
        public string Username { get; set; }

        [DisplayName("Forename:")]
        public string Forename { get; set; }

        [DisplayName("Surname:")]
        public string Surname { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Birth:")]
        public DateTime? Dob { get; set; }

        [DisplayName("ID:")]
        public int Id { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Added:")]
        public DateTime? Dateadd { get; set; }

        [DisplayName("Marketing news letter")]
        public bool MarketingNewsLetter { get; set; }

        [DisplayName("Marketing third party")]
        public bool MarketingThirdParty { get; set; }

        [DisplayName("Address 1:")]
        [MaxLength(128)]
        public string Address1 { get; set; }

        [DisplayName("Address 2:")]
        [MaxLength(128)]
        public string Address2 { get; set; }

        [MaxLength(128)]
        [DisplayName("Address 3:")]
        public string Address3 { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Town { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string District { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string PostCode { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CountryCode { get; set; }
        public List<SelectListItem> CountryItems { get; set; }

        [MaxLength(16)]
        [DisplayName("Phone Number:")]
        public string Phone { get; set; }

        [MaxLength(512)]
        [DisplayName("Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool MultipleCardExists { get; set; } = false;

        public bool ExpiredCardExists { get; set; } = false;

        public List<CustomerDetailsMembershipCardsViewModel> MembershipCardList { get; set; }


        public CustomerDetailsViewModel()
        {
            MembershipCardList = new List<CustomerDetailsMembershipCardsViewModel>();      
        }
    }
}