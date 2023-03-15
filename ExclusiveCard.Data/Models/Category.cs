using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("Category", Schema = "Exclusive")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(1024)]
        [DataType("nvarchar")]
        public string Code { get; set; }

        public int ParentId { get; set; }

        //[MaxLength(50)]
        //[DataType("nvarchar")]
        //public string UrlSlack { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
        [MaxLength(60)]
        [DataType("nvarchar")]
        public string UrlSlug { get; set; }

        public virtual ICollection<OfferCategory> OfferCategories { get; set; }
        public virtual ICollection<CategoryFeatureDetail> CategoryFeatureDetails { get; set; }
    }
}
