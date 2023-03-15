using dto= ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ExclusiveCard.Services.Models.DTOs
{
  public  class MarketingCampaign
    {
        public int Id { get; set; }
        public int WhiteLabelId { get; set; }
        public int NewsletterId { get; set; }

        public string CampaignReference { get; set; }

        public string CampaignName { get; set; }

        public int SenderId { get; set; }

        public bool Enabled { get; set; }

        public virtual WhiteLabelSettings WhiteLabel { get; set; }
        public virtual Newsletter Newsletter { get; set; }

        //public virtual NewsletterCampaignLink NewsletterCampaignLink { get; set; }

    }

}
