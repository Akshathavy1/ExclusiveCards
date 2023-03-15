using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using db = ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Services.Public
{
    [Obsolete("Functionality needs moving to CashbackManager")]
    public class CashbackPayoutService : ICashbackPayoutService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ICashbackPayoutManager _cashbackPayoutManager;

        #endregion

        #region Constructor

        public CashbackPayoutService(IMapper mapper, ICashbackPayoutManager cashbackPayoutManager)
        {
            _mapper = mapper;
            _cashbackPayoutManager = cashbackPayoutManager;
        }

        #endregion

        #region Writes

        //Add CashbackPayout
        public async Task<CashbackPayout> Add(CashbackPayout cashbackPayout)
        {
            db.CashbackPayout req = MapCashbackPayoutReq(cashbackPayout);
            var resp = await _cashbackPayoutManager.Add(req);

            return MapCashbackPayoutDto(resp);
        }

        //Update CashbackPayout
        public async Task<CashbackPayout> Update(CashbackPayout cashbackPayout)
        {
            db.CashbackPayout req = MapCashbackPayoutReq(cashbackPayout);
            var resp = await _cashbackPayoutManager.Update(req);

            return MapCashbackPayoutDto(resp);
        }

        #endregion

        #region Reads

        //Get CashbackPayout with Id
        public async Task<CashbackPayout> Get(int id)
        {
            return MapCashbackPayoutDto(await _cashbackPayoutManager.Get(id));
        }
        //Get CashbackPayout with customerId, partnerId, currencyCode
        public async Task<CashbackPayout> GetByCustomerPartnerCurrency(int customerId, string currencyCode)
        {
            return MapCashbackPayoutDto(await _cashbackPayoutManager.GetByCustomerPartnerCurrency(customerId, currencyCode));
        }

        ////manual mappings to get all cashbackpayout
        //public async Task<List<CashbackPayout>> GetAllAsync()
        //{
        //    return ManualMappings.MapCashbackPayouts(
        //         await _cashbackPayoutManager.GetAll());
        //}

        //public async Task<CashbackPayout> GetByMembershipCardId(int membershipCardId)
        //{
        //    return ManualMappings.MapCashbackPayout(await _cashbackPayoutManager.GetByMembershipCard(membershipCardId));
        //}

        public async Task<WithdrawalRequestModel> GetCashoutDataForRequest(string userId)
        {
            var resp = await _cashbackPayoutManager.GetCashoutDataForRequest(userId);
            var dtoResult = _mapper.Map<WithdrawalRequestModel>(resp);
            return dtoResult;
        }

        public async Task<PagedResult<CashbackPayout>> GetCashbackPaidoutData(int statusId, int page, int pageSize, Enums.WithdrawalSortOrder sortOrder)
        {
            var resp = await _cashbackPayoutManager.GetCashbackPaidoutData(statusId, page, pageSize, sortOrder);

            var result = _mapper.Map<PagedResult<CashbackPayout>>(resp);

            return result;
            //return ManualMappings.MapPagedCashbackPayout(resp);
        }

        public  PagedResult<FinancialReportSummary> GetPagedFinancialReportSearch(int statusId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var list = _cashbackPayoutManager.GetPagedFinancialReportSearch(statusId, startDate, endDate, page, pageSize);
            return Map(list, page, pageSize);
        }
        public PagedResult<FinancialReportSummary> GetAllPagedFinancialReport(DateTime startDate, DateTime endDate)
        {
            var list = _cashbackPayoutManager.GetAllPagedFinancialReport(startDate, endDate);
            return Map(list,0,0);
        }
        #endregion
        private PagedResult<FinancialReportSummary> Map(List<db.SPFinancialReport> spFinancialReport, int page, int pageSize)
        {
            PagedResult<FinancialReportSummary> result = new PagedResult<FinancialReportSummary>();
            foreach (var item in spFinancialReport)
            {
                var report = new FinancialReportSummary()
                {
                
                    Beneficiary = item.Beneficiary,
                    BeneficiaryCommission = item.BeneficiaryCommission,
                    TalkSportCommission = item.TalkSportCommission,
                    Description = item.Description,
                    ExclusiveCommission = item.ExclusiveCommission,
                    CashbackAmount=item.CashbackAmount,
                    ClickCount=item.ClickCount,
                    CustomerCount=item.CustomerCount
                    
                };
                
                result.Results.Add(report);
            }

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = Convert.ToInt32(spFinancialReport.FirstOrDefault()?.TotalRecord);
            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            return result;
        }

        private db.CashbackPayout MapCashbackPayoutReq(CashbackPayout data)
        {
            if (data == null)
                return null;
            return new db.CashbackPayout
            {
                Id = data.Id,
                CustomerId = data.CustomerId,
                StatusId = data.StatusId,
                PayoutDate = data.PayoutDate,
                Amount = data.Amount,
                CurrencyCode = data.CurrencyCode,
                BankDetailId = data.BankDetailId,
                RequestedDate = data.RequestedDate,
                ChangedDate = data.ChangedDate,
                UpdatedBy = data.UpdatedBy
            };
        }

        private CashbackPayout MapCashbackPayoutDto(db.CashbackPayout data)
        {
            if (data == null)
                return null;
            var dto = new CashbackPayout
            {
                Id = data.Id,
                CustomerId = data.CustomerId,
                StatusId = data.StatusId,
                PayoutDate = data.PayoutDate,
                Amount = data.Amount,
                CurrencyCode = data.CurrencyCode,
                BankDetailId = data.BankDetailId,
                RequestedDate = data.RequestedDate,
                ChangedDate = data.ChangedDate,
                UpdatedBy = data.UpdatedBy
            };

            if (data.Customer != null)
            {
                dto.Customer = new Customer
                {
                    Id = data.Customer.Id,
                    AspNetUserId = data.Customer.AspNetUserId,
                    ContactDetailId = data.Customer.ContactDetailId,
                    Title = data.Customer.Title,
                    Forename = data.Customer.Forename,
                    Surname = data.Customer.Surname,
                    DateOfBirth = data.Customer.DateOfBirth,
                    IsActive = data.Customer.IsActive,
                    IsDeleted = data.Customer.IsDeleted,
                    DateAdded = data.Customer.DateAdded,
                    MarketingNewsLetter = data.Customer.MarketingNewsLetter,
                    MarketingThirdParty = data.Customer.MarketingThirdParty,
                    NINumber = data.Customer.NINumber
                };
            }

            if (data.Status != null)
            {
                dto.Status = new Status
                {
                    Id = data.Status.Id,
                    Name = data.Status.Name,
                    Type = data.Status.Type,
                    IsActive = data.Status.IsActive
                };
            }

            if (data.BankDetail != null)
            {
                dto.BankDetail = new BankDetail
                {
                    Id = data.BankDetail.Id,
                    BankName = data.BankDetail.BankName,
                    ContactDetailId = data.BankDetail.ContactDetailId,
                    SortCode = data.BankDetail.SortCode,
                    AccountNumber = data.BankDetail.AccountNumber,
                    AccountName = data.BankDetail.AccountName,
                    IsDeleted = data.BankDetail.IsDeleted
                };
            }

            return dto;
        }

        
    }
}
