using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("ErrorLog", Schema = "Exclusive")]
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ApplicationId { get; set; }
        public int EnvironmentId { get; set; }
        [MaxLength(50)]
        [DataType("nvarchar")]
        public string MachineName { get; set; }
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }
        [MaxLength(5)]
        [DataType("nvarchar")]
        public string Level { get; set; }
        [MaxLength(80)]
        [DataType("nvarchar")]
        public string Logger { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string AssemblyName { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string ClassName { get; set; }
        [MaxLength(100)]
        [DataType("nvarchar")]
        public string MethodName { get; set; }
        [MaxLength(4000)]
        [DataType("nvarchar")]
        public string Message { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string StackTrace { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string InnerMessage{ get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string Exception { get; set; }
    }
}
