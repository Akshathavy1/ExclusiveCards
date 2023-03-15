using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.StagingModels
{
    [Table("CustomerRegistration", Schema = "Staging")]
    public class CustomerRegistration
    {
        [Key]
        public Guid CustomerPaymentId { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Data { get; set; }
        [ForeignKey("CustomerStatus")]
        public int StatusId { get; set; }
        [MaxLength(450)]
        [DataType("nvarchar")]
        public string AspNetUserId { get; set; }
        public virtual Status CustomerStatus { get; set; }
    }
}
