using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Newsletter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Schedule { get; set; }
        public bool Enabled { get; set; }
        public int EmailTemplateId { get; set; }
        public int OfferListId { get; set; }

        public EmailTemplate EmailTemplate { get; set;}
    }
}
