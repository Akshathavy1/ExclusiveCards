using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("CategoryFeatureDetail", Schema = "Exclusive")]
    public class CategoryFeatureDetail
    {
        [Key, Column(Order = 0), ForeignKey("Category")]
        public int CategoryId { get; set; }
        [Key, Column(Order = 1)]
        [DataType("nvarchar")]
        [MaxLength(3)]
        public string CountryCode { get; set; }
        [ForeignKey("Merchant")]
        public int FeatureMerchantId { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string SelectedImage { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string UnselectedImage { get; set; }

        public virtual Category Category { get; set; }
        public virtual Merchant Merchant { get; set; }
    }
}
