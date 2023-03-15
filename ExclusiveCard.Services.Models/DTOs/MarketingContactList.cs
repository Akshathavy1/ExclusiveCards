using ExclusiveCard.Services.Models.DTOs.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MarketingContactList
    {
        public int Id { get; set; }

        public int WhiteLabelId { get; set; }

        public string ContactListReference { get; set; }
        public string ContactListName { get; set; }

        public virtual WhiteLabelSettings WhiteLabelSettings { get; set; }
    }
}
