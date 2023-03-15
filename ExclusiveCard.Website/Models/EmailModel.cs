using System.Collections.Generic;

namespace ExclusiveCard.Website.Models
{
    public class EmailModel
    {

        public string EmailFrom { get; set; }

        public string Subject { get; set; }
        public string BodyPlainText { get; set; }
        public string BodyHtml { get; set; }
        public List<string> EmailTo { get; set; }
        public List<string> EmailCC { get; set; }
        public List<string> EmailBCC { get; set; }

    }
}
