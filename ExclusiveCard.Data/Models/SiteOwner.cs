using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("SiteOwner", Schema = "CMS")]
    public class SiteOwner
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        public string ClanHeading { get; set; }

        public string BeneficiaryHeading { get; set; }

        public string StarndardHeading { get; set; }

        public string ClanDescription { get; set; }
        public string BeneficiaryConfirmation { get; set; }
        public string StarndardRewardConfirmation { get; set; }  

    }
}
