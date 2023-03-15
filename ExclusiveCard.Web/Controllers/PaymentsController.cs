using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Text;
using System.Globalization;

namespace ExclusiveCard.WebAdmin.Controllers
{
    //TODO : 
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class PaymentsController : BaseController
    {
        #region Private Members

        private readonly IPartnerTransactionService _partnerTransactionService;
        private readonly IUserService _userService;
        private readonly Services.Interfaces.Admin.IPartnerService _partnerService;
        private readonly IStatusServices _statusService;
        private readonly IPartnerRewardWithdrawalService _partnerRewardWithdrawalService;
        private readonly ICashbackPayoutService _cashbackPayoutService;

        private readonly IOptions<TypedAppSettings> _settings;

        #endregion

        #region Contructor

        public PaymentsController(IPartnerTransactionService partnerTransactionService,
            IUserService userService,
            Services.Interfaces.Admin.IPartnerService partnerService,
            IStatusServices statusService,
            IPartnerRewardWithdrawalService partnerRewardWithdrawalService,
            ICashbackPayoutService cashbackPayoutService,

            IOptions<TypedAppSettings> settings)
        {
            _partnerTransactionService = partnerTransactionService;
            _userService = userService;
            _partnerService = partnerService;
            _statusService = statusService;
            _partnerRewardWithdrawalService = partnerRewardWithdrawalService;
            _cashbackPayoutService = cashbackPayoutService;

            _settings = settings;
        }

        #endregion

        [HttpGet]
        [ActionName("PartnerTransactions")]
        public async Task<IActionResult> PartnerTransactions(int? statusId, int? partnerId, int? page, string sortField,
            string sortDirection)
        {
            try
            {
                //Initialise
                PartnerTransactionListRequest request = new PartnerTransactionListRequest
                {
                    TransactionSortField = TransactionSortField.Date,
                    SortDirection = "desc"
                };
                if (!string.IsNullOrEmpty(sortField) && sortField == TransactionSortField.FileName.ToString())
                {
                    request.TransactionSortField = TransactionSortField.FileName;
                }

                if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "asc")
                {
                    request.SortDirection = sortDirection;
                }

                SearchPaymentsViewModel model = new SearchPaymentsViewModel
                {
                    PartnerTransactions = new TransactionsList(request)
                };
                if (statusId.HasValue)
                {
                    model.StatusId = statusId.Value;
                }

                if (partnerId.HasValue)
                {
                    model.PartnerId = partnerId.Value;
                }

                if (page.HasValue)
                {
                    model.PartnerTransactions.CurrentPageNumber = page;
                    model.PartnerTransactions.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.PartnerTransactions.CurrentPageNumber = 1;
                    model.PartnerTransactions.PagingModel.CurrentPage = 1;
                }

                //await PaymentsHelper.GetPaymentsViewModel(model, request, _statusService, _partnerService, _partnerTransactionService);
                //Get Payment Status
                model.Status = Enum.GetValues(typeof(FilePayment)).Cast<FilePayment>()
                    .Where(x => x == FilePayment.Paid || x == FilePayment.Unpaid).Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int)v).ToString()
                    }).ToList();

                if (model.StatusId == 0)
                {
                    model.StatusId =
                        (int)FilePayment
                            .Unpaid; //statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.Unpaid).Id;
                }

                //get partners
                var partners = await _partnerService.GetAllAsync((int)PartnerType.RewardPartner);
                if (partners?.Count > 0)
                {
                    foreach (var partner in partners)
                    {
                        model.Partners.Add(new SelectListItem
                        {
                            Text = partner.Name,
                            Value = partner.Id.ToString()
                        });
                    }

                    if (model.PartnerId == 0)
                    {
                        model.PartnerId = partners.FirstOrDefault().Id;
                    }
                }

                TransactionSortOrder sortOrder = GetSortOrder(request);
                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize, out pageSize);
                //Get Transactions
                var data = await _partnerTransactionService.GetTransactionsAsync(model.StatusId,
                    model.PartnerId, (int)model.PartnerTransactions.CurrentPageNumber, pageSize, sortOrder);
                //Map to ViewModel
                //MapToViewModel(model.PartnerTransactions, data);
                if (data != null)
                {
                    foreach (var result in data.Results)
                    {
                        model.PartnerTransactions.Transactions.Add(new PartnerTransactionsViewModel
                        {
                            Id = result.Id,
                            FileName = result.Name,
                            CreatedDate = result.CreatedDate,
                            Amount = $"£{result.TotalAmount?.ToString()}",
                            PaymentStatus = result.PaymentStatus?.Name
                        });
                    }

                    model.PartnerTransactions.CurrentPageNumber = data.CurrentPage;
                    model.PartnerTransactions.PagingModel.CurrentPage = data.CurrentPage;
                    model.PartnerTransactions.PagingModel.PageCount = data.PageCount;
                    model.PartnerTransactions.PagingModel.PageSize = data.PageSize;
                    model.PartnerTransactions.PagingModel.RowCount = data.RowCount;
                }

                //sortDirection and SortField

                if (request.TransactionSortField == TransactionSortField.Date)
                {
                    model.PartnerTransactions.SortIcon = request.SortDirection == "desc"
                        ? "fa fa-sort-numeric-desc"
                        : "fa fa-sort-numeric-asc";
                }
                else if (request.TransactionSortField == TransactionSortField.FileName)
                {
                    model.PartnerTransactions.SortIcon = request.SortDirection == "desc"
                        ? "fa fa-sort-alpha-desc"
                        : "fa fa-sort-alpha-asc";
                }

                return View("PartnerTransactions", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction(int fileId)
        {
            try
            {
                if (fileId == 0)
                {
                    return Json(JsonResponse<string>.ErrorResponse("File not found"));
                }

                IdentityUser user = await _userService.GetUserAsync(HttpContext.User);
                //call helper to update the transaction
                var response = false;
                Services.Models.DTOs.Files file = await _partnerTransactionService.GetByIdAsync(fileId);
                if (file != null)
                {
                    file.PaidDate = DateTime.UtcNow;
                    file.ChangedDate = DateTime.UtcNow;
                    file.UpdatedBy = user.Id;
                    file.PaymentStatusId = (int)FilePayment.Paid; //statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.Paid).Id;
                    var resp = await _partnerTransactionService.UpdateAsync(file);
                    if (resp != null && resp.PaidDate == file.PaidDate)
                        response = true;
                }

                if (response)
                    return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return Json(JsonResponse<string>.ErrorResponse("Error updating the partner transaction to paid."));
        }

        [HttpGet]
        [ActionName("RewardWithdrawal")]
        public async Task<IActionResult> RewardWithdrawal(int? statusId, int? partnerId, int? page, string sortField,
            string sortDirection)
        {

            try
            {

                //Initialise
                RewardWithdrawalListRequest request = new RewardWithdrawalListRequest
                {
                    WithdrawalSortField = WithdrawalSortField.CustomerName,
                    SortDirection = "asc"
                };
                if (!string.IsNullOrEmpty(sortField) && sortField == WithdrawalSortField.CustomerName.ToString())
                {
                    request.WithdrawalSortField = WithdrawalSortField.CustomerName;
                }

                if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "desc")
                {
                    request.SortDirection = sortDirection;
                }

                SearchWithdrawalsViewModel model = new SearchWithdrawalsViewModel
                {
                    WithdrawalsList = new WithdrawalsList(request)
                };
                if (statusId.HasValue)
                {
                    model.StatusId = statusId.Value;
                }

                if (partnerId.HasValue)
                {
                    model.PartnerId = partnerId.Value;
                }

                if (page.HasValue)
                {
                    model.WithdrawalsList.CurrentPageNumber = page;
                    model.WithdrawalsList.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.WithdrawalsList.CurrentPageNumber = 1;
                    model.WithdrawalsList.PagingModel.CurrentPage = 1;
                }

                //await PaymentsHelper.GetRewardWithdrawalsViewModel(model, request, _statusService, _partnerService, _partnerRewardWithdrawalService);
                //Get Payment Status

                model.Status = Enum.GetValues(typeof(WithdrawalStatus)).Cast<WithdrawalStatus>()
                    .Where(x => x == WithdrawalStatus.Paid || x == WithdrawalStatus.Confirmed).Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int)v).ToString()
                    }).ToList();

                if (model.StatusId == 0)
                {
                    model.StatusId = (int)WithdrawalStatus.Confirmed;//statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.Confirmed).Id;
                }

                //get partners
                var partners = await _partnerService.GetAllAsync((int)PartnerType.RewardPartner);
                if (partners?.Count > 0)
                {
                    foreach (var partner in partners)
                    {
                        model.Partners.Add(new SelectListItem
                        {
                            Text = partner.Name,
                            Value = partner.Id.ToString()
                        });
                    }

                    if (model.PartnerId == 0)
                    {
                        model.PartnerId = partners.FirstOrDefault().Id;
                    }
                }

                WithdrawalSortOrder sortOrder = GetSortOrder(request);
                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize, out pageSize);
                //Get Transactions
                var data = await _partnerRewardWithdrawalService.GetWithdrawalsForPayments(model.PartnerId, model.StatusId,
                    (int)model.WithdrawalsList.CurrentPageNumber, pageSize, sortOrder);

                //Map to ViewModel
                if (data != null)
                {
                    foreach (var result in data.Results)
                    {
                        if (result.PartnerReward.MembershipCards.Count>0)
                        {
                            var customer = result.PartnerReward.MembershipCards.FirstOrDefault().Customer;
                            var bankDetail = customer.CustomerBankDetails.FirstOrDefault().BankDetail;
                            model.WithdrawalsList.Withdrawals.Add(item: new WithdrawalViewModel()
                            {
                                PartnerRewardId = (int)customer.MembershipCards.FirstOrDefault().PartnerRewardId,
                                Name = $"{customer?.Forename} {customer?.Surname}",
                                AccountName = bankDetail.AccountName,
                                AccountNumber = bankDetail.AccountNumber,
                                SortCode = bankDetail.SortCode,
                                ConfirmedAmount = result.ConfirmedAmount ?? 0m,
                                Status = Enum.GetName(typeof(WithdrawalStatus), result.StatusId),
                                Id = result.Id
                            });
                        }

                       
                    }

                    model.WithdrawalsList.CurrentPageNumber = data.CurrentPage;
                    model.WithdrawalsList.PagingModel.CurrentPage = data.CurrentPage;
                    model.WithdrawalsList.PagingModel.PageCount = data.PageCount;
                    model.WithdrawalsList.PagingModel.PageSize = data.PageSize;
                    model.WithdrawalsList.PagingModel.RowCount = data.RowCount;
                }

                //sortDirection and SortField
                model.WithdrawalsList.SortIcon = request.SortDirection == "desc"
                    ? "fa fa-sort-alpha-desc"
                    : "fa fa-sort-alpha-asc";

                return View("RewardWithdrawals", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("UpdateRewardWithdrawal")]
        public async Task<IActionResult> UpdateRewardWithdrawal(int withdrawalId)
        {
            try
            {
                if (withdrawalId == 0)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Withdrawal request not found"));
                }

                IdentityUser user = await _userService.GetUserAsync(HttpContext.User);

                //call helper to update the transaction
                bool response = false;
                var req = await _partnerRewardWithdrawalService.GetByIdAsync(withdrawalId);
                if (req != null)
                {
                    //var statuses = await _statusService.GetAll(Data.Constants.StatusType.WithdrawalStatus);
                    req.ChangedDate = DateTime.UtcNow;
                    req.UpdatedBy = user.Id;
                    req.StatusId = (int)WithdrawalStatus.Paid;//statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.Paid).Id;
                    var resp = await _partnerRewardWithdrawalService.UpdateAsync(req);
                    if (resp != null && resp.ChangedDate == req.ChangedDate)
                        response = true;
                }

                if (response)
                    return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return Json(JsonResponse<string>.ErrorResponse("Error updating the partner transaction to paid."));
        }

        [HttpGet]
        [ActionName("CustomerWithdrawal")]
        public async Task<IActionResult> CustomerWithdrawal(int? statusId, int? page, string sortField,
            string sortDirection, string startDate, string endDate)
        {
            try
            {
                //Initialise


                RewardWithdrawalListRequest request = new RewardWithdrawalListRequest
                {
                    WithdrawalSortField = WithdrawalSortField.CustomerName,
                    SortDirection = "asc"
                };
                if (!string.IsNullOrEmpty(sortField) && sortField == WithdrawalSortField.CustomerName.ToString())
                {
                    request.WithdrawalSortField = WithdrawalSortField.CustomerName;
                }

                if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "desc")
                {
                    request.SortDirection = sortDirection;
                }

                SearchWithdrawalsViewModel model = new SearchWithdrawalsViewModel
                {
                    WithdrawalsList = new WithdrawalsList(request)
                };
                //if (!string.IsNullOrEmpty(startDate))
                //{
                //    model.StartDate = Convert.ToDateTime(startDate);
                //}
                //if (!string.IsNullOrEmpty(endDate))
                //{
                //    model.EndDate = Convert.ToDateTime(endDate);
                //}
                if (statusId.HasValue)
                {
                    model.StatusId = statusId.Value;
                }

                if (page.HasValue)
                {
                    model.WithdrawalsList.CurrentPageNumber = page;
                    model.WithdrawalsList.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.WithdrawalsList.CurrentPageNumber = 1;
                    model.WithdrawalsList.PagingModel.CurrentPage = 1;
                }

                //await PaymentsHelper.GetWithdrawalsViewModel(model, request, _statusService, _cashbackPayoutService);
                //Get Payment Status
                model.Status = Enum.GetValues(typeof(Cashback)).Cast<Cashback>()
                    .Where(x => x == Cashback.PaidOut || x == Cashback.Requested).Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int)v).ToString()
                    }).ToList();

                if (model.StatusId == 0)
                {
                    model.StatusId =
                        (int)Cashback
                            .Requested; //statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.Requested).Id;
                }

                WithdrawalSortOrder sortOrder = GetSortOrder(request);
                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize, out pageSize);

                var data = await _cashbackPayoutService.GetCashbackPaidoutData(model.StatusId,
                   (int)model.WithdrawalsList.CurrentPageNumber, pageSize, sortOrder);

                //Map to ViewModel
                if (data != null)
                {
                    foreach (var result in data.Results)
                    {
                        var bankDetail = result.BankDetail;
                        model.WithdrawalsList.Withdrawals.Add(new WithdrawalViewModel()
                        {
                            Name = $"{result.Customer?.Forename} {result.Customer?.Surname}",
                            AccountNumber = bankDetail.AccountNumber,
                            AccountName = bankDetail.AccountName,
                            SortCode = bankDetail.SortCode,
                            ConfirmedAmount = result.Amount,
                            Status = Enum.GetName(typeof(Cashback), result.StatusId),
                            Id = result.Id,
                            PayOutDate = result.PayoutDate

                        });
                    }

                    model.WithdrawalsList.CurrentPageNumber = data.CurrentPage;
                    model.WithdrawalsList.PagingModel.CurrentPage = data.CurrentPage;
                    model.WithdrawalsList.PagingModel.PageCount = data.PageCount;
                    model.WithdrawalsList.PagingModel.PageSize = data.PageSize;
                    model.WithdrawalsList.PagingModel.RowCount = data.RowCount;
                }
                //sortDirection and SortField

                model.WithdrawalsList.SortIcon = request.SortDirection == "desc"
                    ? "fa fa-sort-alpha-desc"
                    : "fa fa-sort-alpha-asc";

                return View("CustomerWithdrawal", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("UpdateWithdrawal")]
        public async Task<IActionResult> UpdateWithdrawal(int withdrawalId)
        {
            try
            {
                if (withdrawalId == 0)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Withdrawal request not found"));
                }

                IdentityUser user = await _userService.GetUserAsync(HttpContext.User);
                //call helper to update the transaction
                //var resp = await PaymentsHelper.UpdateWithdrawal(withdrawalId, user.Id, _cashbackPayoutService, _statusService);
                var response = false;

                var req = await _cashbackPayoutService.Get(withdrawalId);
                if (req != null)
                {
                    req.ChangedDate = DateTime.UtcNow;
                    req.UpdatedBy = user.Id;
                    req.CurrencyCode = "GBP";
                    req.PayoutDate = DateTime.UtcNow;
                    req.StatusId = (int)Cashback.PaidOut; //statuses.FirstOrDefault(x => x.Name == Data.Constants.Status.PaidOut).Id;
                    var resp = await _cashbackPayoutService.Update(req);
                    if (resp != null && resp.ChangedDate == req.ChangedDate)
                        response = true;
                }

                if (response)
                    return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return Json(JsonResponse<string>.ErrorResponse("Error updating the partner transaction to paid."));
        }


        [HttpGet]
        [ActionName("FinancialReport")]
        public IActionResult FinancialReport(int? page,string startDate, string endDate)
        {
            try
            {
                //Initialise

                
                SearchFinancialReportViewModel model = new SearchFinancialReportViewModel
                {
                    FinancialReport = new FinancialReport()
                };
                if (!string.IsNullOrEmpty(startDate))
                {
                    model.StartDate = Convert.ToDateTime(startDate);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    model.EndDate = Convert.ToDateTime(endDate);
                }

                if (page.HasValue)
                {
                    model.FinancialReport.CurrentPageNumber = page;
                    model.FinancialReport.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.FinancialReport.CurrentPageNumber = 1;
                    model.FinancialReport.PagingModel.CurrentPage = 1;
                }

                if (model.StatusId == 0)
                {
                    model.StatusId =
                        (int)Cashback
                            .Received;
                }

                int pageSize = 1;
                int.TryParse(_settings.Value.PageSize, out pageSize);

                var data =  _cashbackPayoutService.GetPagedFinancialReportSearch(model.StatusId, model.StartDate, model.EndDate,
                   (int)model.FinancialReport.CurrentPageNumber, pageSize);

                var dsr = _cashbackPayoutService.GetAllPagedFinancialReport(model.StartDate, model.EndDate);

                if (data != null)
                {
                    foreach (var result in data.Results)
                    {
                        model.FinancialReport.FinancialReportViewModel.Add(new FinancialReportViewModel()
                        {
                            Description = result.Description,
                            Beneficiary = result.Beneficiary,
                            BeneficiaryCommission = result.BeneficiaryCommission,
                            TalkSportCommission = result.TalkSportCommission,
                            ExclusiveCommission = result.ExclusiveCommission,
                            ClickCount=result.ClickCount,
                            CustomerCount=result.CustomerCount,
                            CashbackAmount=result.CashbackAmount

                        });
                    }

                    model.FinancialReport.CurrentPageNumber = data.CurrentPage;
                    model.FinancialReport.PagingModel.CurrentPage = data.CurrentPage;
                    model.FinancialReport.PagingModel.PageCount = data.PageCount;
                    model.FinancialReport.PagingModel.PageSize = data.PageSize;
                    model.FinancialReport.PagingModel.RowCount = data.RowCount;
                }
                //sortDirection and SortField

                //model.WithdrawalsList.SortIcon = request.SortDirection == "desc"
                //    ? "fa fa-sort-alpha-desc"
                //    : "fa fa-sort-alpha-asc";

                return View("FinancialReport", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }


        [HttpGet]
        [ActionName("DownloadFinancialReport")]
        public IActionResult DownloadFinancialReport(DateTime startDate, DateTime endDate, int rowCount)
        {
            try
            {
                if (rowCount > 0)
                {
                    var start = startDate;
                    var end = endDate;
                    string fileName =
                        $"FinancialReport_{start.Year}{start.Month}{start.Day}_{end.Year}{end.Month}{end.Day}.csv";

                    var data = _cashbackPayoutService.GetPagedFinancialReportSearch(0, start, end, 1, rowCount);


                    if (data != null && data.RowCount > 0)
                    {
                        //Get csv string
                        StringBuilder csv = new StringBuilder();
                        //build header
                        csv.Append($"Description,");
                        csv.Append($"Beneficiary,");
                        csv.Append($"Beneficiary Commission,");
                        csv.Append($"TalkSport Commission,");
                        csv.Append($"Exclusive Commission,");
                      
                        csv.Append("\r\n");

                        foreach (var result in data.Results)
                        {
                            csv.Append($"{result.Description}");
                            csv.Append($",{result.Beneficiary}");
                            csv.Append($",{result.BeneficiaryCommission}");
                            csv.Append($",{result.TalkSportCommission}");
                            csv.Append($",{result.ExclusiveCommission}");
                            csv.Append("\r\n");
                        }

                        if (string.IsNullOrEmpty(csv.ToString()))
                        {
                            return Json(JsonResponse<string>.ErrorResponse("No data found to export into csv."));
                        }

                        byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
                        return File(buffer, "text/csv", fileName);
                    }
                    else
                    {
                        return Json(JsonResponse<string>.ErrorResponse("No data found to export into csv."));
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Report not found."));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error creating csv file."));
            }
        }
        #region Private Methods

        private TransactionSortOrder GetSortOrder(PartnerTransactionListRequest req)
        {
            TransactionSortOrder sortOrder;

            switch (req.TransactionSortField)
            {
                case TransactionSortField.Date:
                    sortOrder = req.SortDirection == "asc" ? TransactionSortOrder.DateAsc : TransactionSortOrder.DateDesc;
                    break;

                case TransactionSortField.FileName:
                    sortOrder = req.SortDirection == "asc"
                        ? TransactionSortOrder.FileNameAsync
                        : TransactionSortOrder.FileNameDesc;
                    break;

                default:
                    sortOrder = TransactionSortOrder.DateDesc;
                    break;
            }

            return sortOrder;
        }

        private WithdrawalSortOrder GetSortOrder(RewardWithdrawalListRequest req)
        {
            WithdrawalSortOrder sortOrder;

            switch (req.WithdrawalSortField)
            {
                case WithdrawalSortField.CustomerName:
                    sortOrder = req.SortDirection == "asc" ? WithdrawalSortOrder.CustomerNameDesc : WithdrawalSortOrder.CustomerNameAsc;
                    break;

                default:
                    sortOrder = WithdrawalSortOrder.CustomerNameAsc;
                    break;
            }

            return sortOrder;
        }

        #endregion
    }
}