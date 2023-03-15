using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MerchantBranch", Schema = "Exclusive")]
    public class MerchantBranch
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ContactDetail")]
        public int? ContactDetailsId { get; set; }
        
        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string ShortDescription { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string LongDescription { get; set; }

        [MaxLength(200)]
        [DataType("nvarchar")]
        public string Notes { get; set; }

        [DataType("nvarchar")]
        public bool Mainbranch { get; set; }

        public short DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Merchant Merchant { get; set; }

        public virtual ContactDetail ContactDetail { get; set; }

        public virtual ICollection<OfferMerchantBranch> OfferMerchantBranches { get; set; }
    }
}
