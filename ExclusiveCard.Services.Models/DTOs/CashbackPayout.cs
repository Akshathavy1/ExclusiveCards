using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
   public class CashbackPayout
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int StatusId { get; set; }

        public DateTime? PayoutDate { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CurrencyCode { get; set; }

        public int? BankDetailId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ChangedDate { get; set; }
        public string UpdatedBy { get; set; }

        public BankDetail BankDetail { get; set; }
        public Status Status { get; set; }
        public Customer Customer { get; set; }

        
    }
}
