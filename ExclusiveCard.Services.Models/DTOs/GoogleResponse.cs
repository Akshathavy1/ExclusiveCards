using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs
{
    //Need to pass the exclusive PaymentNotificationId id around with the response
    public class GoogleResponse : GoogleSubscriptionResponce
    {
        //Exclusive notification Id
        public int PaymentNotificationId { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"PaymentNotificationId:{PaymentNotificationId}, ");
            sb.Append($"StartTimeMillis:{StartTimeMillis}, ");
            sb.Append($"PromotionType:{PromotionType}, ");
            sb.Append($"PromotionCode:{PromotionCode}, ");
            sb.Append($"PriceCurrencyCode:{PriceCurrencyCode}, ");
            sb.Append($"PriceAmountMicros:{PriceAmountMicros}, ");
            sb.Append($"PaymentState:{PaymentState}, ");
            sb.Append($"OrderId:{OrderId}, ");
            sb.Append($"UserCancellationTimeMillis:{UserCancellationTimeMillis}, ");
            sb.Append($"LinkedPurchaseToken:{LinkedPurchaseToken}, ");
            sb.Append($"ExpiryTimeMillis:{ExpiryTimeMillis}, ");
            sb.Append($"DeveloperPayload:{DeveloperPayload}, ");
            sb.Append($"CountryCode:{CountryCode}, ");
            sb.Append($"CancelReason:{CancelReason}, ");
            sb.Append($"AutoRenewing:{AutoRenewing}, ");
            sb.Append($"AcknowledgementState:{AcknowledgementState}, ");
            sb.Append($"Kind:{Kind}, ");
            sb.Append($"ETag:{ETag}");
            return sb.ToString();
        }
    }

    public class GoogleSubscriptionResponce
    {
        //
        // Summary:
        //     Time at which the subscription was granted, in milliseconds since the Epoch.
        public virtual long? StartTimeMillis { get; set; }
        //
        // Summary:
        //     The type of purchase of the subscription. This field is only set if this purchase
        //     was not made using the standard in-app billing flow. Possible values are: 0.
        //     Test (i.e. purchased from a license testing account) 1. Promo (i.e. purchased
        //     using a promo code)
        //public virtual int? PurchaseType { get; set; }
        //
        // Summary:
        //     The type of promotion applied on this purchase. This field is only set if a promotion
        //     is applied when the subscription was purchased. Possible values are: 0. One time
        //     code 1. Vanity code
        public virtual int? PromotionType { get; set; }
        //
        // Summary:
        //     The promotion code applied on this purchase. This field is only set if a vanity
        //     code promotion is applied when the subscription was purchased.
        public virtual string PromotionCode { get; set; }
        //
        // Summary:
        //     ISO 4217 currency code for the subscription price. For example, if the price
        //     is specified in British pounds sterling, price_currency_code is "GBP".
        public virtual string PriceCurrencyCode { get; set; }
        //
        // Summary:
        //     Price of the subscription, not including tax. Price is expressed in micro-units,
        //     where 1,000,000 micro-units represents one unit of the currency. For example,
        //     if the subscription price is €1.99, price_amount_micros is 1990000.
        public virtual long? PriceAmountMicros { get; set; }
        //
        // Summary:
        //     The payment state of the subscription. Possible values are: 0. Payment pending
        //     1. Payment received 2. Free trial 3. Pending deferred upgrade/downgrade Not present
        //     for canceled, expired subscriptions.
        public virtual int? PaymentState { get; set; }
        //
        // Summary:
        //     The order id of the latest recurring order associated with the purchase of the
        //     subscription.
        public virtual string OrderId { get; set; }
        //
        // Summary:
        //     The time at which the subscription was canceled by the user, in milliseconds
        //     since the epoch. Only present if cancelReason is 0.
        public virtual long? UserCancellationTimeMillis { get; set; }
        //
        // Summary:
        //     The purchase token of the originating purchase if this subscription is one of
        //     the following: 0. Re- signup of a canceled but non-lapsed subscription 1. Upgrade/downgrade
        //     from a previous subscription For example, suppose a user originally signs up
        //     and you receive purchase token X, then the user cancels and goes through the
        //     resignup flow (before their subscription lapses) and you receive purchase token
        //     Y, and finally the user upgrades their subscription and you receive purchase
        //     token Z. If you call this API with purchase token Z, this field will be set to
        //     Y. If you call this API with purchase token Y, this field will be set to X. If
        //     you call this API with purchase token X, this field will not be set.
        public virtual string LinkedPurchaseToken { get; set; }
        //
        // Summary:
        //     Time at which the subscription will expire, in milliseconds since the Epoch.
        public virtual long? ExpiryTimeMillis { get; set; }
        //
        // Summary:
        //     A developer-specified string that contains supplemental information about an
        //     order.
        public virtual string DeveloperPayload { get; set; }
        //
        // Summary:
        //     ISO 3166-1 alpha-2 billing country/region code of the user at the time the subscription
        //     was granted.
        public virtual string CountryCode { get; set; }
        //
        // Summary:
        //     The reason why a subscription was canceled or is not auto-renewing. Possible
        //     values are: 0. User canceled the subscription 1. Subscription was canceled by
        //     the system, for example because of a billing problem 2. Subscription was replaced
        //     with a new subscription 3. Subscription was canceled by the developer
        public virtual int? CancelReason { get; set; }
        //
        // Summary:
        //     Whether the subscription will automatically be renewed when it reaches its current
        //     expiry time.
        public virtual bool? AutoRenewing { get; set; }
        //
        // Summary:
        //     The acknowledgement state of the subscription product. Possible values are: 0.
        //     Yet to be acknowledged 1. Acknowledged
        public virtual int? AcknowledgementState { get; set; }
        //
        // Summary:
        //     This kind represents a subscriptionPurchase object in the androidpublisher service.
        public virtual string Kind { get; set; }
        //
        // Summary:
        //     The ETag of the item.
        public virtual string ETag { get; set; }
    }

}
