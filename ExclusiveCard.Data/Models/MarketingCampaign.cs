using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ExclusiveCard.Data.Models
{
    [Table("Campaigns", Schema = "Marketing")]
    public class MarketingCampaign
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("WhiteLabelSettings")]
        public int WhiteLabelId { get; set; }

        [ForeignKey("Newsletter")]
        public int NewsletterId { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string CampaignReference { get; set; }

        [MaxLength(100)]
        [DataType("nvarchar")]
        public string CampaignName { get; set; }

        public int? SenderId { get; set; }

        public bool Enabled { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }

        public virtual Newsletter Newsletter { get; set; }

    }
}