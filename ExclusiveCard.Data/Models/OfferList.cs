using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("OfferLists", Schema = "CMS")]
    public class OfferList
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string ListName { get; set; }

        [MaxLength(500)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public int MaxSize { get; set; }

        public bool IsActive { get; set; }

        public bool IncludeShowAllLink { get; set; }
        public int? DisplayType { get; set; }

        public short PermissionLevel { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ShowAllLinkCaption { get; set; }

        [ForeignKey("WhiteLabelSettings")]
        public int? WhitelabelId { get; set; }

        public virtual ICollection<OfferListItem> OfferListItems { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }
    }
}