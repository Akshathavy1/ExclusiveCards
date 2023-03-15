using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class EmailsSent
    {
            public int Id { get; set; }
            public int EmailTemplateId { get; set; }
            public int CustomerId { get; set; }
            public string EmailTo { get; set; }
            public DateTime DateSent { get; set; }

            public virtual EmailTemplate EmailTemplate { get; set; }
            public virtual Customer Customer { get; set; }
    }
}
