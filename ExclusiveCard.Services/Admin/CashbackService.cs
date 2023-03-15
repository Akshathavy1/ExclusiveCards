using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces.Admin;
using AutoMapper;
using ExclusiveCard.Data.Constants;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using NLog;
using pService = ExclusiveCard.Services.Interfaces.Public;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Enums;
using System.Linq.Expressions;

namespace ExclusiveCard.Services.Admin
{
    public class CashbackService : ICashbackService
    {
        #region Private Members

        private readonly IAffiliateMappingService _affiliateMappingService;
        private readonly IAffiliateService _affiliateService;
        private readonly IAffiliateMappingRuleService _affiliateMappingRuleService;
        private readonly IMembershipCardAffiliateReferenceService _membershipCardAffiliateReferenceService;
        private readonly ICashbackManager _cashbackManager;
        private readonly IStagingCashbackManager _stagingCashbackManager;

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public CashbackService( IAffiliateMappingService affiliateMappingService, 
                                IAffiliateService affiliateService, 
                                IAffiliateMappingRuleService affiliateMappingRuleService,
                                IMembershipCardAffiliateReferenceService membershipCardAffiliateReferenceService,
                                IStagingCashbackManager stagingCashbackManager, 
                                ICashbackManager cashbackManager)
        {
            
            _affiliateMappingService = affiliateMappingService;
            _affiliateService = affiliateService;
            _affiliateMappingRuleService = affiliateMappingRuleService;
            _membershipCardAffiliateReferenceService = membershipCardAffiliateReferenceService;
            _stagingCashbackManager = stagingCashbackManager;
            _cashbackManager = cashbackManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        /// <summary>
        /// Downloads the cashback transactions report from Strackr API
        /// This returns a list of all cashback transactions that have been tracked by exclusive affililates 
        /// between a specified date range. 
        /// Report includes new transactions, and status updates to existing transactions. 
        /// Due to design "feature", the date range is searched on date Added,  not date last amended
        /// Therefore we have to pass in 90 day range, to avoid missing any status updates 
        /// Uploads the data into Staging.CashbackTransactions table
        /// Creates a file record in the Staging.TrasactionFile table
        /// </summary>
        /// <param name="dateFrom">Start Date</param>
        /// <param name="dateTo">End Date</param>
        /// <param name="apiId">Strackr API Id</param>
        /// <param name="apiKey">Strackr API Key </param>
        /// <returns></returns>
        public int GetTransactionReport(DateTime? dateFrom, DateTime? dateTo, string apiId, string apiKey)
        {
            int transactionFileId = 0;

            if (!dateFrom.HasValue || !dateTo.HasValue)
                throw new Exception ("Required DateFrom and DateTo values.");

            string fileName = "CashbackTransactions-" + (DateTime.Now).ToString("yyyyMMdd") + "-" + (DateTime.Now).ToString("HHmmss") + ".json";
            dto.StagingModels.TransactionFile transactionFile = new dto.StagingModels.TransactionFile
            {
                FileName = fileName,
                DateFrom = (DateTime)dateFrom,
                DateTo = (DateTime)dateTo,
                StatusId = (int) StagingTransactionFiles.Uploading 
            };

            string path = Path.Combine(Directory.GetCurrentDirectory(), "JsonFile/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var json = DownloadJsonFile(transactionFile, path, apiId, apiKey);

            if (json != null)
            {
                //Save to Staging.TransactionFiles
                transactionFileId = _stagingCashbackManager.CreateTransactionFile(transactionFile);

                if (transactionFileId != 0)
                {
                    CreateStagingTransactionsFromJson(json, path, transactionFileId);
                }

            }

            return transactionFileId;
        }

        public async Task<string> MigrateCashbackTransactions(string adminEmail, string cashbackConfirmedInDays)
        {
            try
            {
                var files = await _stagingCashbackManager.GetTransactionFilesAsync(StagingTransactionFiles.Uploaded);
                if (files != null && files.Count > 0)
                {
                    //Get all Affiliate Mappings
                    var affiliateMappings = await _affiliateMappingService.GetAll();

                    //Get Mapping rules
                    List<dto.AffiliateMappingRule> mappingRule =
                        await _affiliateMappingRuleService.GetAllMappingRules("Merchants");

                    //Get Affiliates
                    var affiliates = await _affiliateService.GetAll();

                    //Get all Membership card affiliate Reference
                    var membershipCardAffiliateReferences =
                        await _membershipCardAffiliateReferenceService.GetAll();

                    foreach (dto.StagingModels.TransactionFile tran in files)
                    {
                        //Update to Processing status
                        _stagingCashbackManager.SetTransactionFileStatus(tran.Id, StagingTransactionFiles.Processing);
                        
                        //Get All Transactions in Staging with status as new
                        var transactions = await _stagingCashbackManager.GetTransactionsAsync(StagingCashbackTransactions.New);
                        
                        if (transactions != null && transactions.Count > 0)
                        {
                            //Pick the top 1 record Id and pass to ApplyTransaction method
                            foreach (dto.StagingModels.CashbackTransaction transaction in transactions)
                            {
                                if (transaction != null)
                                {
                                    //Update Cashback Transaction status to In Progress
                                    _stagingCashbackManager.SetTransactionStatus(transaction.Id, StagingCashbackTransactions.InProgress);
                                    
                                    //Call Apply Transaction
                                    string result =
                                        await ApplyTransaction(transaction.Id, transaction, mappingRule,
                                            affiliates, membershipCardAffiliateReferences, affiliateMappings, cashbackConfirmedInDays);
                                }
                                else
                                {
                                    _logger.Error("Cashback transaction not found.");
                                }
                            }
                        }
                        
                        //Update to Processed status
                        _stagingCashbackManager.SetTransactionFileStatus(tran.Id, StagingTransactionFiles.Processed);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return true.ToString();
        }

        #region Private Methods

        private JObject DownloadJsonFile(dto.StagingModels.TransactionFile transactionFile, string path, string apiId, string apiKey)
        {
            JObject json = null;

            try
            {
                
                string url = $"https://api.strackr.com/v3/reports/transactions?api_id={apiId}&api_key={apiKey}&time_start=" + string.Format("{0:yyyy-MM-dd}", transactionFile.DateFrom) + "&time_end=" + string.Format("{0:yyyy-MM-dd}", transactionFile.DateTo);
                var response = "";

                //Download Json File and Create Json File
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    byte[] b = wc.DownloadData(url);
                    MemoryStream output = new MemoryStream();
                    using (GZipStream g = new GZipStream(new MemoryStream(b), CompressionMode.Decompress))
                    {
                        g.CopyTo(output);
                    }
                    response = Encoding.UTF8.GetString(output.ToArray());
                    json = JObject.Parse(response);

                    if (!string.IsNullOrEmpty(response))
                    {
                        File.WriteAllText(path + transactionFile.FileName, json.ToString());
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Error calling Strackr API", ex);
            }
            
            return json;
        }

        private void CreateStagingTransactionsFromJson(JObject json, string path, int transactionFileId)
        {
            JArray errorTransactionJsonFile = new JArray();

            //Mapping Json File to Staging.CashbackTransactions Model
            foreach (JObject result in json["results"])
            {
                try
                {
                    if (result["baskets"].Any())
                    {
                        foreach (JObject basket in result["baskets"])
                        {
                            dto.StagingModels.CashbackTransaction cashbackTransaction =
                                new dto.StagingModels.CashbackTransaction
                                {
                                    TransactionFileId = transactionFileId,
                                    ResultsId = result["id"].ToString(),
                                    SourceId = result["source_id"].ToString(),
                                    NetworkId = result["network_id"].ToString(),
                                    NetworkName = result["network_name"].ToString(),
                                    ConnectionId = result["connection_id"].ToString(),
                                    MerchantId = result["advertiser_id"].ToString(),
                                    MerchantName = result["advertiser_name"].ToString(),
                                    OrderId = result["order_id"].ToString(),
                                    Country = result["country"].ToString(),
                                    Referrer = result["referrer"].ToString(),
                                    BasketId = basket["id"].ToString(),
                                    BaskedSourceId = basket["source_id"].ToString(),
                                    Name = basket["name"].ToString(),
                                    Currency = basket["currency"].ToString(),
                                    PriceTotal = (decimal)basket["price_total"],
                                    Revenue = (decimal)basket["revenue"],
                                    SourceCurrency = basket["source_currency"]["currency"].ToString(),
                                    SourcePriceTotal = (decimal)basket["source_currency"]["price_total"],
                                    SourceRevenue = (decimal)basket["source_currency"]["revenue"],
                                    RecordStatusId = (int)Enums.StagingCashbackTransactions.New
                                };
                            if (string.IsNullOrEmpty(result["paid"].ToString()))
                            {
                                cashbackTransaction.Paid = false;
                            }
                            else
                            {
                                cashbackTransaction.Paid = (bool)result["paid"];
                            }
                            if (result["customs"].Any() && !string.IsNullOrEmpty(result["customs"][0].ToString()))
                            {
                                cashbackTransaction.MembershipCardReference = result["customs"][0].ToString();
                            }
                            if (!string.IsNullOrEmpty(result["clicked"].ToString()))
                            {
                                cashbackTransaction.Clicked = (DateTime)result["clicked"];
                            }
                            if (!string.IsNullOrEmpty(result["sold"].ToString()))
                            {
                                cashbackTransaction.Sold = (DateTime)result["sold"];
                            }
                            if (!string.IsNullOrEmpty(result["checked"].ToString()))
                            {
                                cashbackTransaction.Checked = (DateTime)result["checked"];
                            }
                            if (!string.IsNullOrEmpty(result["status_name"].ToString()))
                            {
                                cashbackTransaction.StatusId = GetStatusId(result["status_name"].ToString());
                            }

                            //Save to Staging.CashbackTransactions
                            _stagingCashbackManager.CreateTransaction(cashbackTransaction);
                        }
                    }
                    else
                    {
                        dto.StagingModels.CashbackTransaction cashbackTransaction =
                                new dto.StagingModels.CashbackTransaction
                                {
                                    TransactionFileId = transactionFileId,
                                    ResultsId = result["id"].ToString(),
                                    SourceId = result["source_id"].ToString(),
                                    NetworkId = result["network_id"].ToString(),
                                    NetworkName = result["network_name"].ToString(),
                                    ConnectionId = result["connection_id"].ToString(),
                                    MerchantId = result["advertiser_id"].ToString(),
                                    MerchantName = result["advertiser_name"].ToString(),
                                    OrderId = result["order_id"].ToString(),
                                    Country = result["country"].ToString(),
                                    Referrer = result["referrer"].ToString(),
                                    SourceCurrency = result["source_currency"]["currency"].ToString(),
                                    SourcePriceTotal = (decimal)result["source_currency"]["price"],
                                    SourceRevenue = (decimal)result["source_currency"]["revenue"],
                                    RecordStatusId = (int)StagingCashbackTransactions.New
                                };
                        if (string.IsNullOrEmpty(result["paid"].ToString()))
                        {
                            cashbackTransaction.Paid = false;
                        }
                        else
                        {
                            cashbackTransaction.Paid = (bool)result["paid"];
                        }
                        if (result["customs"].Any() && !string.IsNullOrEmpty(result["customs"][0].ToString()))
                        {
                            cashbackTransaction.MembershipCardReference = result["customs"][0].ToString();
                        }
                        if (!string.IsNullOrEmpty(result["clicked"].ToString()))
                        {
                            cashbackTransaction.Clicked = (DateTime)result["clicked"];
                        }
                        if (!string.IsNullOrEmpty(result["sold"].ToString()))
                        {
                            cashbackTransaction.Sold = (DateTime)result["sold"];
                        }
                        if (!string.IsNullOrEmpty(result["checked"].ToString()))
                        {
                            cashbackTransaction.Checked = (DateTime)result["checked"];
                        }
                        if (!string.IsNullOrEmpty(result["status_name"].ToString()))
                        {
                            cashbackTransaction.StatusId = GetStatusId(result["status_name"].ToString());
                        }

                        //Save to Staging.CashbackTransactions
                        _stagingCashbackManager.CreateTransaction(cashbackTransaction);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    result.Add("errorMessage", ex.Message + ex.StackTrace);
                    errorTransactionJsonFile.Add(result.ToString());
                }
            }

            if (errorTransactionJsonFile?.Count > 0)
            {
                File.WriteAllText(path + "CashbackTransactionError" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.UtcNow) + ".json", errorTransactionJsonFile.ToString());
                throw new Exception("Error while importing files");
            }
            else
            {
                //No Exception then update statusId in Staging.TransactionFile
                _stagingCashbackManager.SetTransactionFileStatus(transactionFileId, StagingTransactionFiles.Uploaded);
            }
        }

        private int GetStatusId(string statusName)
        {
            int statusId = 0;
            switch (statusName.ToLower())
            {
                case "pending":
                    statusId = (int)Cashback.Pending;
                    break;
                case "confirmed":
                    statusId = (int)Cashback.Confirmed;
                    break;
                case "received":
                    statusId = (int)Cashback.Received;
                    break;
                case "declined":
                    statusId = (int)Cashback.Declined;
                    break;
                default:
                    throw new Exception("Unrecognised cashback status,  " + statusName);
            }

            return statusId;
        }

        public async Task<string> ApplyTransaction(int id,
            dto.StagingModels.CashbackTransaction stagingTransaction, List<dto.AffiliateMappingRule> mappingRule,
            List<dto.Affiliate> affiliates,
            List<dto.MembershipCardAffiliateReference> membershipCardAffiliateReferences,
            List<dto.AffiliateMapping> affiliateMappings,  string cashbackConfirmedInDays)
        {
            StringBuilder errorString = new StringBuilder();

            int tempStatusId = 0;
            int statusReceived = (int) Cashback.Received;

            try
            {
                
                if (stagingTransaction == null)
                {
                    errorString = errorString.Append($"Could not find transaction - {id}");
                    errorString = errorString.Append(", ");
                }

                // Find Affiiate Id
                var affiliateId = affiliates.FirstOrDefault(x =>
                    string.Equals(x.Name, stagingTransaction.NetworkName, StringComparison.CurrentCultureIgnoreCase)) == null ? 0 : affiliates.FirstOrDefault(x =>
                    string.Equals(x.Name, stagingTransaction.NetworkName, StringComparison.CurrentCultureIgnoreCase)).Id;

                if (affiliateId == 0)
                {
                    errorString = errorString.Append($"Could not find affiliate for {stagingTransaction.NetworkName} of transaction - {id}");
                    errorString = errorString.Append(", ");
                }

                // Find Merchant Id
                int merchantId = 0;
                var reference = new dto.MembershipCardAffiliateReference();
                if (affiliateId > 0)
                {
                    ////Get Mapping rule for the affiliate (network Name) in transaction
                    dto.AffiliateMappingRule tranMapping = mappingRule?.FirstOrDefault(x => x.AffiliateId == affiliateId);
                    if (tranMapping != null)
                    {
                        //Get MerchantId from AffiliateMapping
                        merchantId = Convert.ToInt32(affiliateMappings.FirstOrDefault(x => x.AffilateValue == stagingTransaction.MerchantName
                                                                                        && x.AffiliateMappingRuleId == tranMapping.Id)?.ExclusiveValue);
                        if (merchantId == 0)
                        {
                            errorString = errorString.Append($"Could not find merchant for transaction - {id}");
                            errorString = errorString.Append(", ");
                        }
                    }
                    else
                    {
                        errorString =
                            errorString.Append($"Could not find merchant mapping rule for {stagingTransaction.NetworkName} of transaction - {id}");
                        errorString = errorString.Append(", ");
                    }

                }

                //Get MembershipCard Details
                if (affiliateId > 0)
                {
                    //Pick up the first populated value from the SubId array, this should be the actual MembershipCardReference
                    var membershipRef = string.IsNullOrWhiteSpace(stagingTransaction.MembershipCardReference)
                                                    ? stagingTransaction.MembershipCardReference
                                                    : Array.Find(stagingTransaction.MembershipCardReference.Split(','), element => !string.IsNullOrWhiteSpace(element));

                    reference = membershipCardAffiliateReferences.FirstOrDefault(x =>
                        x.AffiliateId == affiliateId && x.CardReference == membershipRef);
                    if (reference == null)
                    {
                        errorString = errorString.Append($"Could not find customer for transaction - {id}");
                        errorString = errorString.Append(", ");
                    }

                }

                //Membership Plan
                dto.MembershipPlan membershipPlan = reference?.MembershipCard?.MembershipPlan;
                if (membershipPlan == null)
                {
                    errorString = errorString.Append($"Could not find membership plan for transaction - {id}");
                    errorString = errorString.Append(", ");
                }
                

                if (string.IsNullOrEmpty(errorString.ToString()))
                {

                    //status Pending or confirm
                    dto.CashbackTransaction tran = new dto.CashbackTransaction
                    {
                        AffiliateId = affiliateId,
                        MembershipCardId = reference.MembershipCardId,
                        MerchantId = merchantId,
                        AffiliateTransactionReference = string.IsNullOrEmpty(stagingTransaction.BasketId) ? $"{stagingTransaction.ResultsId}" : $"{stagingTransaction.ResultsId} | {stagingTransaction.BasketId}",
                        TransactionDate = Convert.ToDateTime(stagingTransaction.Sold, new CultureInfo("en-GB")),
                        PurchaseAmount = stagingTransaction.SourcePriceTotal,
                        CurrencyCode = stagingTransaction.SourceCurrency,
                        Detail = TruncateString(stagingTransaction.Name, 70),
                        StatusId = stagingTransaction.StatusId,
                        Summary = TruncateString(stagingTransaction.MerchantName, 30),
                        CashbackAmount = stagingTransaction.SourceRevenue
                        // ExpectedPaymentDate, DateConfirmed, DateReceived, PartnerCashbackPayoutId
                    };
                    if (membershipPlan != null && membershipPlan.PartnerId.HasValue)
                    {
                        tran.PartnerId = membershipPlan.PartnerId.Value;
                    }

                    //Update Status to Received by checking Paid field
                    if (stagingTransaction.Paid && stagingTransaction.StatusId != (int)Cashback.Declined)
                    {
                        tran.StatusId = statusReceived;
                        tempStatusId = stagingTransaction.StatusId;
                        stagingTransaction.StatusId = statusReceived;
                    }

                    string error = string.Empty;
                    if (string.IsNullOrEmpty(errorString.ToString()))
                    {
                        error = await CheckIfExistsAndAddUpdateTransaction(tran, stagingTransaction, membershipPlan,  cashbackConfirmedInDays);
                    }

                    if (error != true.ToString())
                    {
                        errorString = errorString.Append(error);
                        errorString = errorString.Append(", ");
                    }

                    if (tempStatusId > 0)
                    {
                        stagingTransaction.StatusId = tempStatusId;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);

                if (tempStatusId > 0)
                {
                    stagingTransaction.StatusId = tempStatusId;
                }
                stagingTransaction.RecordStatusId = (int)Enums.StagingCashbackTransactions.Failed;
                errorString = errorString.Append($"{e.Message} {e.InnerException} {e.StackTrace}");
            }


            // If an error happend somewhere above
            if (!string.IsNullOrEmpty(errorString.ToString()))
            {
                //write to error handling
                _stagingCashbackManager.SetTransactionStatus(stagingTransaction.Id, StagingCashbackTransactions.Failed);
                AddToErrorHandling(stagingTransaction, errorString.ToString());
            }
            else
            {
                // update transaction record to Complete
                _stagingCashbackManager.SetTransactionStatus(stagingTransaction.Id, StagingCashbackTransactions.Completed);
            }

            // What on earth is the point of returning true regardless, even more, why convert to string?????.  FFS!
            return true.ToString();
        }

        private string TruncateString(string data, int maxSize)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Length < maxSize)
                    maxSize = data.Length;

                data = data.Substring(0, maxSize);
            }

            return data;
        }

        private async Task<string> CheckIfExistsAndAddUpdateTransaction(dto.CashbackTransaction cashbackTran, dto.StagingModels.CashbackTransaction source, 
                                                                        dto.MembershipPlan membershipPlan, string cashbackConfirmedInDays)
        {
            string response = true.ToString();
            try
            {
                //Check if transaction exists
                List<dto.CashbackTransaction> transactionExists = await _cashbackManager.GetCashbackTransactionsByAffiliateRefAsync(cashbackTran.AffiliateTransactionReference);
                if (transactionExists != null && transactionExists.Count > 0)
                {
                    response = await UpdateExistingTxn(cashbackTran, source, membershipPlan, cashbackConfirmedInDays, response,  transactionExists);
                }
                else
                {
                    cashbackTran.StatusId = cashbackTran.StatusId;
                    if (source.StatusId == (int)Cashback.Received)
                    {
                        cashbackTran.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                        cashbackTran.DateReceived = DateTime.UtcNow;
                        cashbackTran.ExpectedPaymentDate = null;
                        cashbackTran.DateConfirmed = null;
                    }
                    else if (source.StatusId == (int)Cashback.Confirmed)
                    {
                        cashbackTran.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                        cashbackTran.ExpectedPaymentDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB")).AddDays(Convert.ToDouble(cashbackConfirmedInDays));
                        cashbackTran.DateConfirmed = source.Checked;
                        cashbackTran.DateReceived = null;
                    }
                    else if (source.StatusId == (int)Cashback.Pending || source.StatusId == (int)Cashback.Declined)
                    {
                        cashbackTran.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                        cashbackTran.ExpectedPaymentDate = null;
                        cashbackTran.DateConfirmed = null;
                        cashbackTran.DateReceived = null;
                    }

                    //add transaction
                    _cashbackManager.CreateCashbackTransactions(cashbackTran, membershipPlan);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                response = $"{ex.Message} Excpetion: {ex.InnerException.ToString()}";
            }
            return response;
        }

        private async Task<string> UpdateExistingTxn(dto.CashbackTransaction cashbackTran, dto.StagingModels.CashbackTransaction source, dto.MembershipPlan membershipPlan, string cashbackConfirmedInDays, string response,  List<dto.CashbackTransaction> transactionExists)
        {
            bool updateSummaryR = false;
            bool updateSummaryD = false;
            bool updateSummaryB = false;

            decimal oldTransactionAmountR = 0m;
            decimal oldTransactionAmountD = 0m;
            decimal oldTransactionAmountB = 0m;

            var tranR = transactionExists.FirstOrDefault(x => x.AccountType == (char)AccountType.AccountTypeR);
            var tranD = transactionExists.FirstOrDefault(x => x.AccountType == (char)AccountType.AccountTypeD);
            var tranB = transactionExists.FirstOrDefault(x => x.AccountType == (char)AccountType.AccountTypeB);

            var amounts = _cashbackManager.GetCashbackValuesForPlan(cashbackTran, membershipPlan);
            decimal amountR = amounts.Item1;
            decimal amountD = amounts.Item2;
            decimal amountB = amounts.Item3;

            if (tranR != null)
            {
                //Check Needs Cashback Summary updation
                updateSummaryR = tranR.CashbackAmount != amountR || tranR.StatusId != cashbackTran.StatusId;
                //Set existing cashback amount in the transaction
                if (updateSummaryR)
                {
                    oldTransactionAmountR = tranR.CashbackAmount;

                    //Set status, date for transaction
                    tranR = SetTransactionFieldValues(source, tranR, cashbackConfirmedInDays);

                    // Update the Cashback Txn and summary 
                    _cashbackManager.UpdateCashbackTransactionStatus(tranR.Id, (Cashback)cashbackTran.StatusId, amountR, tranR.TransactionDate, tranR.DateReceived, tranR.ExpectedPaymentDate, tranR.DateConfirmed);
                }
            }

            if (tranD != null)
            {
                //Check Needs Cashback Summary updation
                updateSummaryD = tranD.CashbackAmount != amountD || tranD.StatusId != cashbackTran.StatusId;
                //Set existing cashback amount in the transaction
                if (updateSummaryD)
                {
                    oldTransactionAmountD = tranD.CashbackAmount;

                    //Set status, date for transaction
                    tranD = SetTransactionFieldValues(source, tranD, cashbackConfirmedInDays);

                    // Update the Cashback Txn and summary 
                    _cashbackManager.UpdateCashbackTransactionStatus(tranD.Id, (Cashback)cashbackTran.StatusId, amountD, tranD.TransactionDate, tranD.DateReceived, tranD.ExpectedPaymentDate, tranD.DateConfirmed);
                }
            }

            if (tranB != null)
            {
                //Check Needs Cashback Summary updation
                updateSummaryB = tranB.CashbackAmount != amountB || tranB.StatusId != cashbackTran.StatusId;
                //Set existing cashback amount in the transaction
                if (updateSummaryB)
                {
                    oldTransactionAmountB = tranB.CashbackAmount;

                    //Set status, date for transaction
                    tranB = SetTransactionFieldValues(source, tranB, cashbackConfirmedInDays);

                    // Update the Cashback Txn and summary 
                    _cashbackManager.UpdateCashbackTransactionStatus(tranB.Id, (Cashback)cashbackTran.StatusId, amountB, tranB.TransactionDate, tranB.DateReceived, tranB.ExpectedPaymentDate, tranB.DateConfirmed);
                }
            }

            await Task.CompletedTask;

            return response;
        }

        //TO set cashback status and dates for the cashbackTransactions while updating
        private dto.CashbackTransaction SetTransactionFieldValues(dto.StagingModels.CashbackTransaction source,
            dto.CashbackTransaction destination, string cashbackConfirmedInDays)
        {
            try
            {
                destination.StatusId = source.StatusId;
                if (source.StatusId == (int)Cashback.Received)
                {
                    destination.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                    destination.DateReceived = DateTime.UtcNow;
                    destination.ExpectedPaymentDate = null;
                    destination.DateConfirmed = null;
                }
                else if (source.StatusId == (int)Cashback.Confirmed)
                {
                    destination.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                    destination.ExpectedPaymentDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"))
                        .AddDays(Convert.ToDouble(cashbackConfirmedInDays));
                    destination.DateConfirmed = source.Checked;
                    destination.DateReceived = null;
                }
                else if ((source.StatusId == (int)Cashback.Pending || source.StatusId == (int)Cashback.Declined) &&
                         destination.StatusId == (int)Cashback.Declined)
                {
                    destination.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                    destination.ExpectedPaymentDate = null;
                    destination.DateConfirmed = null;
                    destination.DateReceived = null;
                }
                else if (source.StatusId == (int)Cashback.Pending &&
                         (destination.StatusId == (int)Cashback.Confirmed || destination.StatusId == (int)Cashback.Received))
                {
                    destination.StatusId = source.StatusId;
                    destination.TransactionDate = Convert.ToDateTime(source.Sold, new CultureInfo("en-GB"));
                    destination.ExpectedPaymentDate = null;
                    destination.DateConfirmed = null;
                    destination.DateReceived = null;
                }
            }
            catch (Exception e)
            {
                destination = null;
                _logger.Error(e);
            }

            return destination;
        }

        private void AddToErrorHandling(dto.StagingModels.CashbackTransaction reqTran, string error)
        {
            dto.StagingModels.CashbackTransactionError errorHandling = new dto.StagingModels.CashbackTransactionError
            {
                TransactionFileId = reqTran.TransactionFileId,
                ResultsId = reqTran.ResultsId,
                SourceId = reqTran.SourceId,
                NetworkId = reqTran.NetworkId,
                NetworkName = reqTran.NetworkName,
                ConnectionId = reqTran.ConnectionId,
                MerchantId = reqTran.MerchantId,
                MerchantName = reqTran.MerchantName,
                OrderId = reqTran.OrderId,
                Country = reqTran.Country,
                Clicked = reqTran.Clicked,
                Sold = reqTran.Sold,
                Checked = reqTran.Checked,
                Referrer = reqTran.Referrer,
                BasketId = reqTran.BasketId,
                BaskedSourceId = reqTran.BaskedSourceId,
                Name = reqTran.Name,
                Currency = reqTran.Currency,
                PriceTotal = reqTran.PriceTotal,
                Revenue = reqTran.Revenue,
                SourceCurrency = reqTran.SourceCurrency,
                SourcePriceTotal = reqTran.SourcePriceTotal,
                SourceRevenue = reqTran.SourceRevenue,
                StatusId = reqTran.StatusId,
                MembershipCardReference = reqTran.MembershipCardReference,
                ProcessedDateTime = DateTime.UtcNow,
                ErrorMessage = error
            };

            _stagingCashbackManager.CreateError(errorHandling);
        }

        

        

        #endregion
    }
}
