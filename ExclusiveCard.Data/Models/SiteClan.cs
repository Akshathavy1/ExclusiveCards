using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("SiteClan", Schema = "CMS")]
    public class SiteClan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("League")]
        public int LeagueId { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string ImagePath { get; set; }

        [MaxLength(10)]
        [DataType("nvarchar")]
        public string PrimaryColour { get; set; }

        [MaxLength(10)]
        [DataType("nvarchar")]
        public string SecondaryColour { get; set; }

        [ForeignKey("Charity")]
        public int? CharityId { get; set; }

        [ForeignKey("SiteOwner")]
        public int SiteOwnerId { get; set; }

        [ForeignKey("SiteCategory")]
        public int SiteCategoryId { get; set; }

        [ForeignKey("WhiteLabelSettings")]
        public int WhiteLabelId { get; set; }

        [ForeignKey("MembershipPlan")]
        public int MembershipPlanId { get; set; }

        public virtual League League { get; set; }

        public virtual Charity Charity { get; set; }

        public virtual SiteOwner SiteOwner { get; set; }

        public virtual SiteCategory SiteCategory { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }
        public virtual MembershipPlan MembershipPlan { get; set; }

    }
}
