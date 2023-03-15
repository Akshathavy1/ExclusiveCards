using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public int TemplateTypeId { get; set; }
        public string EmailName { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public string HeaderHtml { get; set; }
        public string FooterHtml { get; set; }
        public bool IsDeleted { get; set; }

        public string HeaderText { get; set; }
        public string FooterText { get; set; }

        public virtual ICollection<EmailsSent> EmailsSent { get; set; }
        public virtual ICollection<Newsletter> Newsletter { get; set; }

        // EmailTemplate(Destination member list)\r\nExclusiveCard.Services.Models.DTOs.EmailTemplate->ExclusiveCard.Data.Models.EmailTemplate
        //(Destination member list)\r\n\r\nUnmapped properties:\r\nHeaderText\r\nFooterText\r\nEmailsSent\r\nNewsletter\r\n"}

    }
}