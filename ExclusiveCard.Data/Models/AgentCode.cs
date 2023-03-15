using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("AgentCode", Schema = "Exclusive")]
    public class AgentCode
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(16)]
        public string ReportCode { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public decimal? CommissionPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
