using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class EmailCustomField
    {
        public int Id { get; set; }

        public string ExclusiveField { get; set; }

        public string CustomName { get; set; }

        public string FieldType { get; set; }

        public string SubstitutionTag { get; set; }

        public string SenderId { get; set; }
    }
}
