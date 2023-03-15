using System.Collections.Generic;
namespace ExclusiveCard.Services.Models.DTOs
{
    public class SiteOwner
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ClanHeading { get; set; }

        public string BeneficiaryHeading { get; set; }

        public string StarndardHeading { get; set; }

        public string ClanDescription { get; set; }
        public string BeneficiaryConfirmation { get; set; }
        public string StarndardRewardConfirmation { get; set; }
    }
}
