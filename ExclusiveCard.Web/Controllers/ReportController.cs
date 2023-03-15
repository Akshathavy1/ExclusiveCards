using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.Models;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class ReportController : BaseController
    {
        #region Private Methods

        private readonly IPartnerTransactionService _partnerTransactionService;
        private readonly IPartnerService _partnerService;
        private readonly IOptions<TypedAppSettings> _settings;

        #endregion

        #region Constructor

        public ReportController(IPartnerTransactionService partnerTransactionService,
            IPartnerService partnerService,
            IOptions<TypedAppSettings> settings)
        {
            _partnerTransactionService = partnerTransactionService;
            _partnerService = partnerService;
            _settings = settings;
        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string sortField, string sortDirection, int partnerId, string startDate,
            string endDate, int? page)
        {
            InvestmentSearchViewModel model = new InvestmentSearchViewModel();
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                {
                    model.StartDate = Convert.ToDateTime(startDate);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    model.EndDate = Convert.ToDateTime(endDate);
                }
                if (partnerId > 0)
                {
                    model.PartnerId = partnerId;
                }

                //Initialise
                TransactionRequest request = new TransactionRequest
                {
                    TransactionSortField = PartnerTransactionSortField.CreatedDate,
                    SortDirection = "asc"
                };
                if (!string.IsNullOrEmpty(sortField) && sortField == PartnerTransactionSortField.Name.ToString())
                {
                    request.TransactionSortField = PartnerTransactionSortField.Name;
                }
                else if (!string.IsNullOrEmpty(sortField) && sortField == PartnerTransactionSortField.PaidDate.ToString())
                {
                    request.TransactionSortField = PartnerTransactionSortField.PaidDate;
                }
                else if (!string.IsNullOrEmpty(sortField) && sortField == PartnerTransactionSortField.FileStatus.ToString())
                {
                    request.TransactionSortField = PartnerTransactionSortField.FileStatus;
                }
                else if (!string.IsNullOrEmpty(sortField) && sortField == PartnerTransactionSortField.PaymentStatus.ToString())
                {
                    request.TransactionSortField = PartnerTransactionSortField.PaymentStatus;
                }

                if (!string.IsNullOrEmpty(sortDirection) && sortDirection == "desc")
                {
                    request.SortDirection = sortDirection;
                }

                model.Transactions = new TransactionsViewModel(request);

                if (page.HasValue)
                {
                    model.Transactions.CurrentPageNumber = page;
                    model.Transactions.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.Transactions.CurrentPageNumber = 1;
                    model.Transactions.PagingModel.CurrentPage = 1;
                }

                string error = string.Empty;

                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize, out pageSize);
                TransactionSortOrder sortOrder = GetSortOrder(request);
                //Get Default Partner (TAM as only one exists)
                var partners = await _partnerService.GetAllAsync((int)PartnerType.RewardPartner);

                if (partners?.Count != null && partners.FirstOrDefault()?.Id > 0)
                {
                    foreach (var partner in partners)
                    {
                        model.Partners.Add(new SelectListItem
                        {
                            Text = partner.Name,
                            Value = partner.Id.ToString()
                        });
                    }

                    model.PartnerId = partners.FirstOrDefault().Id;
                    var data = await _partnerTransactionService.GetTransactionsAsync(model.PartnerId,
                        model.StartDate, model.EndDate, (int)model.Transactions.CurrentPageNumber,
                        pageSize, sortOrder);

                    if (data != null)
                    {
                        model.Transactions.Transactions.AddRange(data.Results.Select(result => new TransactionViewModel
                        {
                            Id = result.Id,
                            CreatedDate = result.CreatedDate,
                            Amount = result.Type == FileType.PartnerTrans.ToString()
                                ? $"£{result.ConfirmedAmount?.ToString(CultureInfo.InvariantCulture)}"
                                : $"-£{result.ConfirmedAmount?.ToString(CultureInfo.InvariantCulture)}",
                            Reference = $"{result.Id}-{result.Name}",
                            Payee = partners?.FirstOrDefault()?.Name,
                            Description = result.Type == FileType.PartnerTrans.ToString()
                                ? "Customer Investments"
                                : "Customer Withdrawals"
                        }));

                        model.Transactions.CurrentPageNumber = data.CurrentPage;
                        model.Transactions.PagingModel.CurrentPage = data.CurrentPage;
                        model.Transactions.PagingModel.PageCount = data.PageCount;
                        model.Transactions.PagingModel.PageSize = data.PageSize;
                        model.Transactions.PagingModel.RowCount = data.RowCount;
                    }

                    model.Transactions.SortIcon = request.SortIcon;
                }
                else
                {
                    error = "Partner not found.";
                    Logger.Error(error);
                }

                if (!error.Equals(string.Empty))
                {
                    ErrorViewModel e = new ErrorViewModel
                    {
                        RequestId = error
                    };
                    return View("Error", e);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View("Investment", model);
        }

        [HttpGet]
        [ActionName("DownloadPayment")]
        public async Task<IActionResult> DownloadPayment(int Id, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (Id > 0)
                {
                    var start = startDate;
                    var end = endDate;
                    var file = await _partnerTransactionService.GetByIdAsync(Id);
                    var partner = await _partnerService.GetByIdAsync(Convert.ToInt32(file.PartnerId));
                    string fileName =
                        $"{partner?.Name}_Payments_{start.Year}{start.Month}{start.Day}_{end.Year}{end.Month}{end.Day}.csv";

                    if (file != null)
                    {
                        //Get csv string
                        StringBuilder csv = new StringBuilder();
                        csv.Append("Date,Amount,Payee,Description,Reference");
                        csv.Append("\r\n");


                        csv.Append($"{file.CreatedDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)},");
                        if (file.Type == FileType.PartnerTrans.ToString())
                        {
                            csv.Append($"£{file.ConfirmedAmount},{partner?.Name},Customer Investments,");
                        }
                        else if (file.Type == FileType.PartnerWithdraw.ToString())
                        {
                            csv.Append($"-£{file.ConfirmedAmount},{partner?.Name},Customer Withdrawals,");
                        }

                        csv.Append($"{file.Id}-{file.Name}");
                        csv.Append("\r\n");


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
                    return Json(JsonResponse<string>.ErrorResponse("Partner not found."));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error creating csv file."));
            }
        }

        [HttpGet]
        [ActionName("PartnerWithdrawal")]
        public async Task<IActionResult> PartnerWithdrawal(string startDate, string endDate, int? page)
        {
            InvestmentSearchViewModel model = new InvestmentSearchViewModel();
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                {
                    model.StartDate = Convert.ToDateTime(startDate);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    model.EndDate = Convert.ToDateTime(endDate);
                }


                //Initialise
                TransactionRequest request = new TransactionRequest
                {
                    TransactionSortField = PartnerTransactionSortField.CreatedDate,
                    SortDirection = "asc"
                };

                model.Transactions = new TransactionsViewModel(request);

                if (page.HasValue)
                {
                    model.Transactions.CurrentPageNumber = page;
                    model.Transactions.PagingModel.CurrentPage = page.Value;
                }
                else
                {
                    model.Transactions.CurrentPageNumber = 1;
                    model.Transactions.PagingModel.CurrentPage = 1;
                }

                //var error = await ReportHelper.MapAndGetPartnerWithdrawalModel(model, request, _partnerService, _partnerTransactionService, Logger);
                string error = string.Empty;

                int pageSize = 50;
                int.TryParse(_settings.Value.PageSize, out pageSize);
                //TransactionSortOrder sortOrder = GetSortOrder(request);
                var data = await _partnerTransactionService.GetPagedCustomerRewardWithdrawalsAsync(
                    model.StartDate, model.EndDate, (int)model.Transactions.CurrentPageNumber,
                    pageSize);

                if (data != null)
                {
                    model.Transactions.CustomerWithdrawals.AddRange(data.Results.Select(result => new CustomerWithdrawViewModel
                    {
                        ContactName = result.ContactName,
                        EmailAddress = result.EmailAddress,
                        InvoiceNumber = result.InvoiceNumber,
                        InvoiceDate = result.InvoiceDate,
                        DueDate = result.DueDate,
                        Total = result.Total,
                        Description = result.Description,
                        Quantity = result.Quantity,
                        UnitAmount = result.UnitAmount,
                        AccountCode = result.AccountCode,
                        TaxType = result.TaxType,
                        TaxAmount = result.TaxAmount,
                        Currency = result.Currency
                    }));

                    model.Transactions.CurrentPageNumber = data.CurrentPage;
                    model.Transactions.PagingModel.CurrentPage = data.CurrentPage;
                    model.Transactions.PagingModel.PageCount = data.PageCount;
                    model.Transactions.PagingModel.PageSize = data.PageSize;
                    model.Transactions.PagingModel.RowCount = data.RowCount;
                }


                if (!error.Equals(string.Empty))
                {
                    ErrorViewModel e = new ErrorViewModel
                    {
                        RequestId = error
                    };
                    return View("Error", e);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View("Withdrawal", model);
        }

        [HttpGet]
        [ActionName("DownloadPartnerWithdraw")]
        public async Task<IActionResult> DownloadPartnerWithdraw(int partnerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (partnerId > 0)
                {
                    var start = startDate;
                    var end = endDate;
                    var partner = await _partnerService.GetByIdAsync(partnerId);
                    string fileName =
                        $"Customer_{partner?.Name}_Withdrawals_{start.Year}{start.Month}{start.Day}_{end.Year}{end.Month}{end.Day}.csv";

                    var data = await _partnerTransactionService.GetCustomerRewardWithdrawalsAsync(partnerId,
                        Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
                    if (data != null && data.Count > 0)
                    {
                        //Get csv string
                        StringBuilder csv = new StringBuilder();
                        //build header
                        csv.Append($"{nameof(CustomerWithdrawViewModel.ContactName)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.EmailAddress)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POAddressLine1)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POAddressLine2)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POAddressLine3)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POAddressLine4)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POCity)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.PORegion)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POPostalCode)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.POCountry)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.InvoiceNumber)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.InvoiceDate)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.DueDate)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.Total)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.InventoryItemCode)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.Description)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.Quantity)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.UnitAmount)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.AccountCode)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TaxType)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TaxAmount)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TrackingName1)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TrackingOption1)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TrackingName2)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.TrackingOption2)},");
                        csv.Append($"{nameof(CustomerWithdrawViewModel.Currency)}");
                        csv.Append("\r\n");

                        foreach (var result in data)
                        {
                            csv.Append($"{result.ContactName},{result.EmailAddress},");
                            csv.Append($",,,,,,,,{result.InvoiceNumber},{result.InvoiceDate},{result.DueDate},£{result.Total},");
                            csv.Append($",{result.Description},{result.Quantity},£{result.UnitAmount},{result.AccountCode},");
                            csv.Append($"{result.TaxType},{result.TaxAmount},,,,,{result.Currency}");
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
                    return Json(JsonResponse<string>.ErrorResponse("Partner not found."));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error creating csv file."));
            }
        }

        #region Private Methods

        private TransactionSortOrder GetSortOrder(TransactionRequest req)
        {
            TransactionSortOrder sortOrder;

            switch (req.TransactionSortField)
            {
                case PartnerTransactionSortField.CreatedDate:
                    sortOrder = req.SortDirection == "asc" ? TransactionSortOrder.DateAsc : TransactionSortOrder.DateDesc;
                    req.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-numeric-desc" : "fa fa-sort-numeric-asc";
                    break;

                case PartnerTransactionSortField.PaidDate:
                    sortOrder = req.SortDirection == "asc" ? TransactionSortOrder.PaidDateAsync : TransactionSortOrder.PaidDateDesc;
                    req.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-numeric-desc" : "fa fa-sort-numeric-asc";
                    break;

                case PartnerTransactionSortField.Name:
                    sortOrder = req.SortDirection == "asc"
                        ? TransactionSortOrder.FileNameAsync
                        : TransactionSortOrder.FileNameDesc;
                    req.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-alpha-desc" : "fa fa-sort-alpha-asc";
                    break;

                case PartnerTransactionSortField.FileStatus:
                    sortOrder = req.SortDirection == "asc" ? TransactionSortOrder.FileStatusAsync : TransactionSortOrder.FileStatusDesc;
                    req.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-alpha-desc" : "fa fa-sort-alpha-asc";
                    break;

                case PartnerTransactionSortField.PaymentStatus:
                    sortOrder = req.SortDirection == "asc" ? TransactionSortOrder.PaymentStatusAsync : TransactionSortOrder.PaymentStatusDesc;
                    req.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-alpha-desc" : "fa fa-sort-alpha-asc";
                    break;

                default:
                    sortOrder = TransactionSortOrder.DateDesc;
                    break;
            }

            return sortOrder;
        }

        #endregion
    }
}
