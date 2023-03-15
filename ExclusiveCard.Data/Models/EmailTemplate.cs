using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("EmailTemplates")]
    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; }
        public int TemplateTypeId { get; set; }
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string EmailName { get; set; }
        [MaxLength(512)]
        [Column(TypeName = "nvarchar(512)")]
        public string Subject { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string BodyText { get; set; }
        [Column(TypeName = "nvarchar(MAX)")] 
        public string BodyHtml { get; set; }
        public bool IsDeleted { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string HeaderText { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string HeaderHtml { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string FooterText { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string FooterHtml { get; set; }


        //public virtual ICollection<EmailAttachment> EmailAttachments { get; set; } Not for this sprint
        public virtual ICollection<EmailsSent> EmailsSent { get; set; } //<< Never used!!!???
        public virtual ICollection<Newsletter> Newsletter { get; set; }
    }
}
