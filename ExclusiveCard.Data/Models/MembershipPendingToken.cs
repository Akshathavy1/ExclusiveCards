
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("MembershipPendingToken",Schema = "Exclusive")]
   public class MembershipPendingToken
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MembershipRegistrationCode")]
        public int MembershipRegistrationCodeId { get; set; }

        public Guid Token { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual MembershipRegistrationCode MembershipRegistrationCode {get; set;}
    }
}
