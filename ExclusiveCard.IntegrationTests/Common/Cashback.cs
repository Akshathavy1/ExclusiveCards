using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Common
{
    public class Cashback
    {
        //TODO:  Refactor Cashback Tests, as old cashback service as been removed

        //public static async Task<List<dto.CashbackTransaction>> CreateTransactions(int membershipCardId, int? partnerId, int statusId)
        //{
        //    var transactionReqs = new List<dto.CashbackTransaction>
        //    {
        //        new dto.CashbackTransaction
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            AffiliateTransactionReference = GenerateReference(10),
        //            TransactionDate = DateTime.UtcNow.AddDays(-4), CashbackAmount = 4.52m, PurchaseAmount = 40m,
        //            CurrencyCode = "GBP", Summary = "Cashback", Detail = "Cashback for offer", StatusId = statusId,
        //            DateReceived = DateTime.UtcNow, PartnerCashbackPayoutId = null
        //        },
        //        new dto.CashbackTransaction
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            AffiliateTransactionReference = GenerateReference(10),
        //            TransactionDate = DateTime.UtcNow.AddDays(-4), CashbackAmount = 3.45m, PurchaseAmount = 28m,
        //            CurrencyCode = "GBP", Summary = "Cashback", Detail = "Cashback for offer", StatusId = statusId,
        //            DateReceived = DateTime.UtcNow, PartnerCashbackPayoutId = null
        //        },
        //        new dto.CashbackTransaction
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            AffiliateTransactionReference = GenerateReference(10),
        //            TransactionDate = DateTime.UtcNow.AddDays(-4), CashbackAmount = 8.12m, PurchaseAmount = 100m,
        //            CurrencyCode = "GBP", Summary = "Cashback", Detail = "Cashback for offer", StatusId = statusId,
        //            DateReceived = DateTime.UtcNow, PartnerCashbackPayoutId = null
        //        },
        //        new dto.CashbackTransaction
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            AffiliateTransactionReference = GenerateReference(10),
        //            TransactionDate = DateTime.UtcNow.AddDays(-4), CashbackAmount = 2.78m, PurchaseAmount = 20m,
        //            CurrencyCode = "GBP", Summary = "Cashback", Detail = "Cashback for offer", StatusId = statusId,
        //            DateReceived = DateTime.UtcNow, PartnerCashbackPayoutId = null
        //        },
        //        new dto.CashbackTransaction
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            AffiliateTransactionReference = GenerateReference(10),
        //            TransactionDate = DateTime.UtcNow.AddDays(-4), CashbackAmount = 5.15m, PurchaseAmount = 70m,
        //            CurrencyCode = "GBP", Summary = "Cashback", Detail = "Cashback for offer", StatusId = statusId,
        //            DateReceived = DateTime.UtcNow, PartnerCashbackPayoutId = null
        //        }
        //    };

        //    var response = new List<dto.CashbackTransaction>();
        //    foreach (var tran in transactionReqs)
        //    {
        //       // response.Add(await ServiceHelper.Instance.CashbackTransactionService.Add(tran));
        //    }

        //    return response;
        //}

        //public static async Task<dto.CashbackSummary> CreateCashbackSummary(int membershipCardId, int? partnerId)
        //{
        //    var summary = new dto.CashbackSummary
        //    {
        //        MembershipCardId = membershipCardId,
        //        PartnerId = partnerId,
        //        AccountType = 'C',
        //        CurrencyCode = "GBP",
        //        PendingAmount = 5m,
        //        ConfirmedAmount = 4.65m,
        //        ReceivedAmount = 24.02m,
        //        FeeDue = 0m,
        //        PaidAmount = 0m
        //    };

        //    return null;
        //    //return await ServiceHelper.Instance.CashbackSummaryService.Add(summary);
        //}

        //public static async Task<List<dto.CashbackSummary>> CreateCashbackSummaries(int membershipCardId, int? partnerId)
        //{

        //    var summaries = new List<dto.CashbackSummary>
        //    {
        //        new dto.CashbackSummary
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'C',
        //            CurrencyCode = "GBP", PendingAmount = 5m, ConfirmedAmount = 4.65m,
        //            ReceivedAmount = 15.78m, FeeDue = 0m, PaidAmount = 0m
        //        },
        //        new dto.CashbackSummary
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'R',
        //            CurrencyCode = "GBP", PendingAmount = 4m, ConfirmedAmount = 2.06m,
        //            ReceivedAmount = 8.24m, FeeDue = 0m, PaidAmount = 0m
        //        },
        //        new dto.CashbackSummary
        //        {
        //            MembershipCardId = membershipCardId, PartnerId = partnerId, AccountType = 'D',
        //            CurrencyCode = "GBP", PendingAmount = 2.35m, ConfirmedAmount = 1.55m,
        //            ReceivedAmount = 4.56m, FeeDue = 0m, PaidAmount = 0m
        //        }
        //    };

        //    var response = new List<dto.CashbackSummary>();
        //    foreach (var summary in summaries)
        //    {
        //        //response.Add(await ServiceHelper.Instance.CashbackSummaryService.Add(summary));
        //    }

        //    return response;
        //}

        //private static string GenerateReference(int length)
        //{
        //    char[] chars =
        //        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        //    byte[] data = new byte[length];
        //    using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        //    {
        //        crypto.GetBytes(data);
        //    }
        //    StringBuilder result = new StringBuilder(length);
        //    foreach (byte b in data)
        //    {
        //        result.Append(chars[b % (chars.Length)]);
        //    }
        //    return result.ToString();
        //}
    }
}
