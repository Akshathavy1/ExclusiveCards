using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class AgentCode
    {
        public int Id { get; set; }
        public string ReportCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? CommissionPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
