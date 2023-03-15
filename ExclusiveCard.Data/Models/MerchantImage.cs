using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("MerchantImage", Schema = "Exclusive")]
   public class MerchantImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string ImagePath { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime TimeStamp { get; set; } = new DateTime(2019, 02, 09, 0, 0, 0, DateTimeKind.Utc);

        public int ImageType { get; set; }

        public virtual Merchant Merchant { get; set; }
    }
}
