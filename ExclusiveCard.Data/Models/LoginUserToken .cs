using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("LoginUserToken ", Schema = "Exclusive")]
    public class LoginUserToken
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IdentityUser")]
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string AspNetUserId { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Token { get; set; }
        public Guid TokenValue { get; set; }

        public virtual ExclusiveUser IdentityUser { get; set; }
    }
}
