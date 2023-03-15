using System;
using System.ComponentModel;

namespace ExclusiveCard.Website.Models
{
    public class AccountViewModel
    {
        public string UserId { get; set; }
        public string PartnerImage { get; set; }
        public string CustomerName { get; set; }
        public string MembershipCardNumber { get; set; }
        
        // Balances
        [DisplayName("Pending")]
        public decimal Pending { get; set; }
        [DisplayName("Confirmed")]
        public decimal Confirmed { get; set; }
        [DisplayName("Received")]
        public decimal Received { get; set; }

      
        [DisplayName("Current Value")]
        public decimal Balance { get; set; }

        [DisplayName("DonatedAmount")]
        public decimal DonatedAmount { get; set; }


        [DisplayName("Invested")]
        public decimal Invested { get; set; }  
        
        [DisplayName("Withdrawn")]
        public decimal Withdrawn { get; set; }


        public string ExpiryDate { get; set; }
        public string MembershipPlanType { get; set; }
        public int PartnerId { get; set; }
        public string PartnerType { get; set; }
        public int MembershipCardId { get; set; }
        public string NiNumber { get; set; }
        public string OneOffPaymentRef { get; set; }
        public Guid CustomerPaymentProviderId { get; set; } 
        public string PayPalLink { get; set; }
        public int MembershipPlanId { get; set; }
        
        public string PartnerPassword { get; set; }

        public TransactionLogList Transactions { get; set; }
        public TransactionLogList Withdrawals { get; set; }
        public WithdrawViewModel Deposit { get; set; }
        public TamViewModel TamDashboard { get; set; }
        public WithdrawViewModel Withdraw { get; set; }
        public SettingsViewModel Settings { get; set; }
        public LayoutViewModel Preferences { get; set; }
      
        
    }
}
