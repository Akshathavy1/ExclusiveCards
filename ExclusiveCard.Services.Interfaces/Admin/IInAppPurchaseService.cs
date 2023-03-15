using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Google.Apis.AndroidPublisher.v3Data;


namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IInAppPurchaseService
    {
        Task<AppleResponse> PostAppleReceipt(bool toLive, AppleReceipt receipt);
        Task<bool> ProcessAppleDiamond(AppleResponse appleResponse, AppleReceipt receipt);

        Task<GoogleResponse> PostGoogleReceipt(GoogleReceipt receipt);
        Task<bool> ProcessGoogleDiamond(GoogleResponse response, GoogleReceipt receipt);

        int ProcessInAppPurchase(PurchaseResult purchase);
    }
}
