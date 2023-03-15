using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("ClickTracking", Schema = "Exclusive")]
    public class ClickTracking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Offer")]
        public int OfferId { get; set; }

        [ForeignKey("MembershipCard")]
        public int MembershipCardId { get; set; }

        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }

        [DataType("nvarchar")]
        [MaxLength(1024)]
        public string DeeplinkURL { get; set; }

        public DateTime DateTime { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual MembershipCard MembershipCard { get; set; }
        public virtual Affiliate Affiliate { get; set; }
    }
}
