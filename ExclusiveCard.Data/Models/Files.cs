using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Files", Schema = "Exclusive")]
    public class Files
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string Name { get; set; }
        [ForeignKey("Partner")]
        public int? PartnerId { get; set; }
        [MaxLength(15)]
        [DataType("nvarchar")]
        public string Type { get; set; }
        [ForeignKey("Status")]
        public int StatusId { get; set; }
        [ForeignKey("PaymentStatus")]
        public int? PaymentStatusId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ChangedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string UpdatedBy { get; set; }
        public decimal? ConfirmedAmount { get; set; }
        [MaxLength(255)]
        [DataType("nvarchar")]
        public string Location { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Status Status { get; set; }
        public virtual Status PaymentStatus { get; set; }
               
        public virtual ICollection<OfferRedemption> OfferRedemptions { get; set; }
    }
}
