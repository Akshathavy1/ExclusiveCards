using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class PurchaseResult
    {
        public Guid UserToken { get; set; }
        public string ItemType { get; set; }
        public string Message { get; set; }
        public bool Successful { get; set; }
        public string Platform { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Currency { get; set; }
        //
        // Summary:
        //     Purchase/Order Id
        public string PurchaseOrderId { get; set; }
        //
        // Summary:
        //     Trasaction date in UTC
        public DateTime TransactionDateUtc { get; set; }
        //
        // Summary:
        //     Product Id/Sku
        public string ProductId { get; set; }
        //
        // Summary:
        //     Indicates whether the subscritpion renewes automatically. If true, the sub is
        //     active, else false the user has canceled.
        public bool AutoRenewing { get; set; }
        //
        // Summary:
        //     Unique token identifying the purchase for a given item
        public string PurchaseToken { get; set; }
        //
        // Summary:
        //     Gets the current purchase/subscription state
        public string State { get; set; }
        //
        // Summary:
        //     Gets the current consumption state
        public string ConsumptionState { get; set; }
        //
        // Summary:
        //     Developer payload
        public string Payload { get; set; }

        public int PaymentProvider { get; set; }
        public int CustomerId { get; set; }

        public override string ToString()
        {
            return $"Platform: {Platform} PurchaseOrderId: {PurchaseOrderId} PaymentAmount: {PaymentAmount} AutoRenewing: {AutoRenewing} PurchaseToken: {PurchaseToken} State: {State} ConsumptionState: {ConsumptionState} PaymentProvider: {PaymentProvider} CustomerId: {CustomerId} UserToken: {UserToken} ItemType: {ItemType} ProductId: {ProductId} Payload: {Payload} ItemType: {ItemType} Message: {Message} Successful: {Successful} TransactionDateUtc: {TransactionDateUtc}";
        }
    }

}
