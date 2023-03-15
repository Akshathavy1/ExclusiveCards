using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("RegistrationCodeSummary")]
    public class RegistrationCodeSummary
    {
        [Key]
        public int Id { get; set; }
        public int MembershipPlanId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int NumberOfCodes { get; set; }
        [MaxLength(512)]
        [DataType("nvarchar")]
        public string StoragePath { get; set; }
        public virtual ICollection<MembershipRegistrationCode> MembershipRegistrationCodes { get; set; }

    }
}
