using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferRedemption", Schema = "Exclusive")]
    public class OfferRedemption
    {
        [Key, Column(Order = 0), ForeignKey("MembershipCard")]
        public int MembershipCardId { get; set; }
        [Key, Column(Order = 1), ForeignKey("Offer")]
        public int OfferId { get; set; }
        [ForeignKey("Status")]
        public int State { get; set; } //(Requested or Complete)
        [ForeignKey("File")]
        public int? FileId { get; set; }//(FK to file table)
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [MaxLength(20)]
        [DataType("nvarchar")]
        public string CustomerRef { get; set; }

        public virtual MembershipCard MembershipCard { get; set; }
        public virtual Offer Offer { get; set; }
        public virtual Status Status { get; set; }
        public virtual Files File { get; set; }
    }
}
