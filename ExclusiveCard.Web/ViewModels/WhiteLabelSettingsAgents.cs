using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class WhiteLabelSettingsAgents
    {
        #region Agents

        [DisplayName("Select Agent")]
        public int Id { get; set; }

        public List<SelectListItem> Agents { get; set; }

        [MaxLength(255)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(16)]
        [DisplayName("Report Code")]
        public string ReportCode { get; set; }

        [Required]
        [MaxLength(255)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [Range(0, 100)]
        [DisplayName("Commission")]
        public int CommissionPercent { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDeleted { get; set; }

        #endregion Agents
    }
}