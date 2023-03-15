using ExclusiveCard.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.StagingModels
{
    [Table("CashbackTransactionErrors", Schema = "Staging")]
    public class CashbackTransactionError
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TransactionFile")]
        public int TransactionFileId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string ResultsId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string SourceId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string NetworkId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string NetworkName { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string ConnectionId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string MerchantId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string MerchantName { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string OrderId { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string Country { get; set; }

        public DateTime? Clicked { get; set; }

        public DateTime? Sold { get; set; }

        public DateTime? Checked { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string Referrer { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string BasketId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string BaskedSourceId { get; set; }

        [MaxLength(300)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(10)]
        [DataType("nvarchar")]
        public string Currency { get; set; }

        public decimal PriceTotal { get; set; }

        public decimal Revenue { get; set; }

        [MaxLength(10)]
        [DataType("nvarchar")]
        public string SourceCurrency { get; set; }

        public decimal SourcePriceTotal { get; set; }

        public decimal SourceRevenue { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }
        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string MembershipCardReference { get; set; }

        public DateTime ProcessedDateTime { get; set; }

        [MaxLength(Int32.MaxValue)]
        [DataType("nvarchar")]
        public string ErrorMessage { get; set; }

        public bool Paid { get; set; }

        public virtual Status Status { get; set; }

        public virtual TransactionFile TransactionFile { get; set; }
    }
}
