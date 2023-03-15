using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("PlanAgentHistory", Schema = "Exclusive")]
    public class PlanAgentHistory
    {
        [Key]
        public int Id { get; set; }
        public int MembershipPlanId { get; set; }
        public int? AgentCodeId { get; set; }
        public DateTime Assigned { get; set; }

    }
}
