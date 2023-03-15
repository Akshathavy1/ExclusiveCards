using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("Newsletters", Schema = "Marketing")]
    public class Newsletter
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Schedule { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("EmailTemplates")]
        public int EmailTemplateId { get; set; }

        [ForeignKey("OfferList")]
        public int OfferListId { get; set; }

        //public virtual ICollection<NewsletterCampaignLink> NewsletterCampaignLink { get; set; }
        public virtual EmailTemplate EmailTemplate { get;set;}


        public Newsletter()
        {
           // NewsletterCampaignLink = new List<NewsletterCampaignLink>();
        }
    }
}
