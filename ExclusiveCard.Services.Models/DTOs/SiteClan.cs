using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class SiteClan
    {
        public int Id { get; set; }

        public int LeagueId { get; set; }

        public string Description { get; set; }

        //public string CharityName { get; set; }

        public string ImagePath { get; set; }

        public string PrimaryColour { get; set; }

        public string SecondaryColour { get; set; }

        public int? CharityId { get; set; }

        public int SiteOwnerId { get; set; }

        public int SiteCategoryId { get; set; }

        public int WhiteLabelId { get; set; }
        public int MembershipPlanId { get; set; }
        public virtual League League { get; set; }

        public virtual Charity Charity { get; set; }

        public virtual SiteOwner SiteOwner { get; set; }

        public virtual SiteCategory SiteCategory { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }
        public virtual MembershipPlan MembershipPlan { get; set; }


    }
}
