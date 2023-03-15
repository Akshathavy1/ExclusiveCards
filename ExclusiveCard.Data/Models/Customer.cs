using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Customer", Schema ="Exclusive")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("IdentityUser")]
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string AspNetUserId { get; set; }


        [ForeignKey("ContactDetail")]
        public int? ContactDetailId { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Title { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Forename { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateAdded { get; set; }

        public bool MarketingNewsLetter { get; set; }

        public bool MarketingThirdParty { get; set; }
        [DataType("nvarchar")]
        [MaxLength(10)]
        public string NINumber { get; set; }

        [ForeignKey("SiteClan")]
        public int? SiteClanId { get; set; }

        public virtual ContactDetail ContactDetail { get; set; }

        public virtual ICollection<CustomerBankDetail> CustomerBankDetails { get; set; }

        public virtual ICollection<CustomerSecurityQuestion> CustomerSecurityQuestions { get; set; }

        public virtual ExclusiveUser IdentityUser { get; set; }

        public virtual ICollection<CashbackPayout> CashbackPayouts { get; set; }

        public virtual ICollection<MembershipCard> MembershipCards { get; set; }

        public virtual ICollection<CustomerPayment> CustomerPayment { get; set; }

        public virtual ICollection<PayPalSubscription> PayPalSubscriptions { get; set; }

        public virtual MarketingContact SendGridContact { get; set; }
        public virtual SiteClan SiteClan { get; set; }
    }
}
