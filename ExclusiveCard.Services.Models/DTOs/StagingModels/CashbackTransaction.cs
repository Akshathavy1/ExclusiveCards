using System;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class CashbackTransaction
    {
        public int Id { get; set; }

        public int TransactionFileId { get; set; }
        
        public string ResultsId { get; set; }

        public string SourceId { get; set; }
       
        public string NetworkId { get; set; }
        
        public string NetworkName { get; set; }
      
        public string ConnectionId { get; set; }
       
        public string MerchantId { get; set; }
        
        public string MerchantName { get; set; }
        
        public string OrderId { get; set; }

        public string Country { get; set; }

        public DateTime? Clicked { get; set; }

        public DateTime? Sold { get; set; }

        public DateTime? Checked { get; set; }

        public string Referrer { get; set; }
     
        public string BasketId { get; set; }
     
        public string BaskedSourceId { get; set; }
        
        public string Name { get; set; }

        public string Currency { get; set; }

        public decimal PriceTotal { get; set; }

        public decimal Revenue { get; set; }

        public string SourceCurrency { get; set; }

        public decimal SourcePriceTotal { get; set; }

        public decimal SourceRevenue { get; set; }

        public int StatusId { get; set; }

        public string MembershipCardReference { get; set; }

        public int RecordStatusId { get; set; }

        public bool Paid { get; set; }

        public Status Status { get; set; }

        public Status RecordStatus { get; set; }

        public TransactionFile TransactionFile { get; set; }
    }
}
