using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Website.Models
{
    public class WithdrawViewModel
    {
        public bool RequestExists { get; set; }
        public int CustomerId { get; set; }
        public int BankDetailId { get; set; }
        public int PartnerRewardId { get; set; }
        [DisplayName("Available Funds")]
        public decimal AvailableFund { get; set; }
        [Required(ErrorMessage = "Withdraw amount is required")]
        [DisplayName("Withdraw Amount *")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public decimal WithdrawAmount { get; set; }
        [DisplayName("Remaining Funds")]
        public decimal RemainingFund { get; set; }
        [Required(ErrorMessage = "Name on account is required")]
        [DisplayName("Name on Account *")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Account number is required")]
        [DisplayName("Account Number *")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Sortcode is required")]
        [DisplayName("Sortcode *")]
        public string SortCode { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Confirm Password *")]
        public string Password { get; set; }
        public string MembershipPlanType { get; set; }
        public int PartnerId { get; set; }
        public string Status { get; set; }
    }
}
