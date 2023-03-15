using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Files
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        public int? PartnerId { get; set; }
        [MaxLength(15)]
        [DataType("nvarchar")]
        public string Type { get; set; }
        public int StatusId { get; set; }
        public int? PaymentStatusId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ChangedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string UpdatedBy { get; set; }
        public decimal? ConfirmedAmount { get; set; }
        public string Location { get; set; }

        public PartnerDto Partner { get; set; }
        public Status Status { get; set; }
        public Status PaymentStatus { get; set; }
        public DateTime CreatedFrom { get; set; }
        public DateTime CreatedTo { get; set; }
    }
}
