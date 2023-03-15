using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    /// <summary>
    /// Base class for receipts on all platforms
    /// </summary>
    public class InAppReceipt
    {
        /// <summary>
        /// The unique ID for your IAP
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The bundle Id of the mobile app
        /// </summary>
        public string BundleId { get; set; }

        /// <summary>
        /// The unique ID for this transaction (OrderId for Google)
        /// </summary>
        public string TransactionId { get; set; }

        public string Customer { get; set; }
    }

    /// <summary>
    /// A receipt for iOS in-app purchases
    /// </summary>
    public class AppleReceipt : InAppReceipt
    {
        /// <summary>
        /// A binary version of the "receipt" from Apple
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// A base64 encoded version "receipt" from Apple
        /// </summary>
        public string Data2 { get; set; }

        public override string ToString()
        {
            //return $"Id: {Id} BundleId: {BundleId} TransactionId: {TransactionId} Data: {BitConverter.ToString(Data)} Data2: {Data2}";
            StringBuilder sb = new StringBuilder();
            sb.Append($"Id:{Id}, ");
            sb.Append($"BundleId:{BundleId}, ");
            sb.Append($"TransactionId:{TransactionId}, ");
            sb.Append($"Customer:{Customer}, ");
            sb.Append($"Data:{BitConverter.ToString(Data)}, ");
            sb.Append($"Data2:{Data2}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// A receipt for Google Play in-app purchases
    /// </summary>
    public class GoogleReceipt : InAppReceipt
    {
        /// <summary>
        /// The "developer payload" used on the purchase
        /// </summary>
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// The receipt data for Google Play. The purchase token for Google, or a signed base64 string for Apple
        /// </summary>
        public string PurchaseToken { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Id:{Id}, ");
            sb.Append($"BundleId:{BundleId}, ");
            sb.Append($"TransactionId:{TransactionId}, ");
            sb.Append($"Customer:{Customer}, ");
            sb.Append($"DeveloperPayload:{DeveloperPayload}, ");
            sb.Append($"PurchaseToken:{PurchaseToken}");
            return sb.ToString();
        }

    }

    public class InAppOrder
    {
        public string PackageName { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public long PurchaseTime { get; set; }
        public int PurchaseState { get; set; }
        public string PurchaseToken { get; set; }
        public bool autoRenewing { get; set; }
        public bool acknowledged { get; set; }
    }
}
