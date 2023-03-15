using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Customer
    {
        public int Id { get; set; }

        public string AspNetUserId { get; set; }

        public int? ContactDetailId { get; set; }

        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string FullName 
        {
            get { return string.Format("{0} {1}", Forename, Surname); }  
        }

        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateAdded { get; set; }

        public bool MarketingNewsLetter { get; set; }

        public bool MarketingThirdParty { get; set; }

        public string NINumber { get; set; }

        public int? SiteClanId { get; set; }

        //TODO:  Add a PartnerReference here

        public ContactDetail ContactDetail { get; set; }

        public ICollection<CustomerBankDetail> CustomerBankDetails { get; set; }

        public ICollection<CustomerSecurityQuestion> CustomerSecurityQuestions { get; set; }

        public ExclusiveUser IdentityUser { get; set; }

        public ICollection<CashbackPayout> CashbackPayouts { get; set; }

        public ICollection<MembershipCard> MembershipCards { get; set; }

        public ICollection<CustomerPayment> CustomerPayment { get; set; }

        public ICollection<PayPalSubscription> PayPalSubscriptions { get; set; }

        public virtual SiteClan SiteClan { get; set; }

    }
}
