using System;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class TamWithdrawalDataModel
    {
        public string TransType { get; set; }
        public string UniqueReference { get; set; }
        public string FundType { get; set; }
        [MaxLength(20)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Forename { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(10)]
        public string NINumber { get; set; }
        public decimal Amount { get; set; }
        public string IntroducerCode { get; set; }
        public string ProcessState { get; set; }
        public int PartnerRewardWithdrawalId { get; set; }
        public int PartnerRewardId { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public int BankDetailId { get; set; }
        public int? FileId { get; set; }
        public DateTime RequestedDate { get; set; }
    }
}
