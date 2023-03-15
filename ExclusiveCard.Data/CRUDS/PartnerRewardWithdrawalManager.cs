using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Constants;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class PartnerRewardWithdrawalManager : IPartnerRewardWithdrawalManager
    {
        #region Private Members

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        #endregion

        #region Contructor

        public PartnerRewardWithdrawalManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion


        #region Writes

        public async Task<PartnerRewardWithdrawal> AddAsync(PartnerRewardWithdrawal data)
        {
            try
            {
                DbSet<PartnerRewardWithdrawal> withdrawals = _ctx.Set<PartnerRewardWithdrawal>();
                await withdrawals.AddAsync(data);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        public async Task<PartnerRewardWithdrawal> UpdateAsync(PartnerRewardWithdrawal data)
        {
            try
            {
                DbSet<PartnerRewardWithdrawal> withdrawals = _ctx.Set<PartnerRewardWithdrawal>();
                withdrawals.Update(data);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        public async Task<List<PartnerRewardWithdrawal>> BulkUpdateAsync(List<PartnerRewardWithdrawal> data)
        {
            try
            {
                DbSet<PartnerRewardWithdrawal> withdrawals = _ctx.Set<PartnerRewardWithdrawal>();
                withdrawals.UpdateRange(data);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        public async Task<PartnerRewardWithdrawal> UpdateErrorAsync(int partnerRewardId, int errorStatus)
        {
            var rec = _ctx.PartnerRewardWithdrawal.AsNoTracking().FirstOrDefault(x =>
                x.PartnerRewardId == partnerRewardId && !x.ConfirmedAmount.HasValue);

            if (rec != null)
            {
                rec.StatusId = errorStatus;

                DbSet<PartnerRewardWithdrawal> rewards = _ctx.Set<PartnerRewardWithdrawal>();
                rewards.Update(rec);
                await _ctx.SaveChangesAsync();
            }

            return rec;
        }

        public async Task<PartnerRewardWithdrawal> UpdateConfirmationAsync(int partnerRewardId, int successStatus, decimal amountConfirmed)
        {
            var rec = _ctx.PartnerRewardWithdrawal.AsNoTracking().FirstOrDefault(x =>
                x.PartnerRewardId == partnerRewardId && (!x.ConfirmedAmount.HasValue || x.ConfirmedAmount == 0m));

            if (rec != null)
            {
                rec.StatusId = successStatus;
                rec.RequestedDate = rec.RequestedDate;
                rec.WithdrawnDate = DateTime.UtcNow;
                rec.ConfirmedAmount = amountConfirmed;
                rec.ChangedDate = DateTime.UtcNow;

                DbSet<PartnerRewardWithdrawal> rewards = _ctx.Set<PartnerRewardWithdrawal>();
                rewards.Update(rec);
                await _ctx.SaveChangesAsync();
            }

            return rec;
        }

        #endregion

        #region Reads

        public async Task<PartnerRewardWithdrawal> GetByIdAsync(int id)
        {
            return await _ctx.PartnerRewardWithdrawal.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PartnerRewardWithdrawal> GetByBankDetailIdAsync(int bankDetailId)
        {
            return await _ctx.PartnerRewardWithdrawal.AsNoTracking().LastOrDefaultAsync(x => x.BankDetailId == bankDetailId);
        }



        public async Task<List<TamWithdrawalDataModel>> GetWithdrawalReport(int statusId)
        {
            List<TamWithdrawalDataModel> dataModels = new List<TamWithdrawalDataModel>();
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetWithdrawalReport]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@StatusId";
                para1.DbType = DbType.Int32;
                para1.Value = statusId;

                cmd.Parameters.Add(para1);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int? fileId;
                            int outFileId;
                            int.TryParse(reader["FileId"].ToString(), out outFileId);
                            if (outFileId == 0)
                                fileId = null;
                            else
                            {
                                fileId = outFileId;
                            }
                            dataModels.Add(new TamWithdrawalDataModel
                            {
                                TransType = reader["TransType"].ToString(),
                                UniqueReference = reader["UniqueReference"].ToString(),
                                FundType = reader["FundType"].ToString(),
                                Title = reader["Title"].ToString(),
                                Forename = reader["Forename"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                NINumber = reader["NINumber"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                IntroducerCode = string.Empty,
                                ProcessState = string.Empty,
                                PartnerRewardWithdrawalId = Convert.ToInt32(reader["PartnerRewardWithdrawalId"]),
                                PartnerRewardId = Convert.ToInt32(reader["PartnerRewardId"]),
                                ConfirmedAmount = string.IsNullOrEmpty(reader["ConfirmedAmount"].ToString()) ? 0 : Convert.ToDecimal(reader["ConfirmedAmount"]),
                                BankDetailId = Convert.ToInt32(reader["BankDetailId"]),
                                FileId = fileId,
                                RequestedDate = Convert.ToDateTime(reader["RequestedDate"].ToString())
                            });
                        }
                    }
                }

                _ctx.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return dataModels;
        }

        public async Task<List<PartnerRewardWithdrawal>> GetAllAsync()
        {
            return await _ctx.PartnerRewardWithdrawal.ToListAsync();
        }

        public async Task<List<PartnerRewardWithdrawal>> GetAllPendingAsync()
        {
            return await _ctx.PartnerRewardWithdrawal
                .Where(x => x.WithdrawalStatus.Type == StatusType.WithdrawalStatus && x.WithdrawalStatus.Name == Constants.Status.Pending).ToListAsync();
        }

        public async Task<WithdrawalRequestModel> GetWithdrawalDataForRequest(int membershipCardId)
        {
            WithdrawalRequestModel data = new WithdrawalRequestModel();
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetRewardWithdrawalData]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@MembershipCardId";
                para1.DbType = DbType.Int32;
                para1.Value = membershipCardId;

                cmd.Parameters.Add(para1);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data = new WithdrawalRequestModel
                            {
                                RequestExists = reader["RequestExists"].ToString() == "1" ? true : false,
                                CustomerId = Convert.ToInt32(reader["CustomerId"].ToString()),
                                BankDetailId = string.IsNullOrEmpty(reader["BankDetailId"].ToString()) ? 0 : Convert.ToInt32(reader["BankDetailId"].ToString()),
                                PartnerRewardId = string.IsNullOrEmpty(reader["PartnerRewardId"].ToString()) ? 0 : Convert.ToInt32(reader["PartnerRewardId"].ToString()),
                                AvailableFund = string.IsNullOrEmpty(reader["AvailableFund"].ToString()) ? decimal.Zero : Convert.ToDecimal(reader["AvailableFund"].ToString()),
                                Name = reader["Name"].ToString(),
                                AccountNumber = reader["AccountNumber"].ToString(),
                                SortCode = reader["SortCode"].ToString()
                            };
                        }
                    }
                }

                _ctx.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return data;
        }

        public async Task<PagedResult<TransactionLogDataModel>> GetTransactionLog(string userId, int page, int pageSize, TransactionLogSortOrder sortOrder)
        {
            PagedResult<TransactionLogDataModel> data = new PagedResult<TransactionLogDataModel>
            {
                PageCount = pageSize,
                CurrentPage = page,
                Results = new List<TransactionLogDataModel>()
            };
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetTransactionLog]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@UserId";
                para1.DbType = DbType.String;
                para1.Value = userId;
                var para2 = cmd.CreateParameter();
                para2.ParameterName = "@PageNumber";
                para2.DbType = DbType.Int32;
                para2.Value = page;
                var para3 = cmd.CreateParameter();
                para3.ParameterName = "@PageSize";
                para3.DbType = DbType.Int32;
                para3.Value = pageSize;
                

                cmd.Parameters.Add(para1);
                cmd.Parameters.Add(para2);
                cmd.Parameters.Add(para3);
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Results.Add(new TransactionLogDataModel()
                            {
                                TotalRecord = Convert.ToInt64(reader["TotalRecord"].ToString()),
                                RowNumber = Convert.ToInt64(reader["RowNumber"].ToString()),
                                Date = Convert.ToDateTime(reader["Date"].ToString()),
                                Merchant = reader["Merchant"].ToString(),
                                Value = reader["Value"].ToString(),
                                Status = reader["Status"].ToString(),
                                Invested = reader["Invested"].ToString() == "1" ? true : false,
                                Donated = reader["Donated"].ToString(),
                                Summary = reader["Summary"].ToString(),
                                PurchaseAmount = reader["PurchaseAmount"].ToString(),
                            });
                        }
                    }
                }

                if (data.Results?.Count > 0)
                {
                    data.RowCount = Convert.ToInt32(data.Results.FirstOrDefault()?.TotalRecord);
                    double pageCount = (double)data.RowCount / pageSize;
                    data.PageCount = (int)Math.Ceiling(pageCount);
                }

                _ctx.Database.CloseConnection();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return data;
        }

        public async Task<PagedResult<TransactionLogDataModel>> GetWithdrawalLog(string userId, int page, int pageSize)
        {
            PagedResult<TransactionLogDataModel> data = new PagedResult<TransactionLogDataModel>
            {
                PageCount = pageSize,
                CurrentPage = page,
                Results = new List<TransactionLogDataModel>()
            };
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetWithdrawalLog]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@UserId";
                para1.DbType = DbType.String;
                para1.Value = userId;
                var para2 = cmd.CreateParameter();
                para2.ParameterName = "@PageNumber";
                para2.DbType = DbType.Int32;
                para2.Value = page;
                var para3 = cmd.CreateParameter();
                para3.ParameterName = "@PageSize";
                para3.DbType = DbType.Int32;
                para3.Value = pageSize;

                cmd.Parameters.Add(para1);
                cmd.Parameters.Add(para2);
                cmd.Parameters.Add(para3);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Results.Add(new TransactionLogDataModel()
                            {
                                Date = Convert.ToDateTime(reader["Date"].ToString()),
                                Merchant = reader["Merchant"].ToString(),
                                Value = reader["Value"].ToString(),
                                Status = reader["Status"].ToString(),
                                Invested = reader["Invested"].ToString() == "1" ? true : false
                            });
                        }
                    }
                }

                if (data.Results?.Count > 0)
                {
                    data.RowCount = Convert.ToInt32(data.Results.FirstOrDefault()?.TotalRecord);
                    double pageCount = (double)data.RowCount / pageSize;
                    data.PageCount = (int)Math.Ceiling(pageCount);
                }

                _ctx.Database.CloseConnection();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return data;
        }

        public async Task<PagedResult<PartnerRewardWithdrawal>> GetWithdrawalsForPayments(int partnerId,
            int status, int page, int pageSize, WithdrawalSortOrder sortOrder)
        {
            IQueryable<PartnerRewardWithdrawal> query = _ctx.PartnerRewardWithdrawal.AsNoTracking()
                .Include(c => c.PartnerReward)
                .Include(c => c.PartnerReward.MembershipCards)
                .ThenInclude(x => x.Customer)
                .ThenInclude(y => y.CustomerBankDetails)
                .ThenInclude(z => z.BankDetail)
                .Where(x => x.PartnerReward.PartnerId == partnerId && x.StatusId == status);

            if (sortOrder == WithdrawalSortOrder.CustomerNameAsc)
            {
                query = query.OrderBy(x => x.PartnerReward.MembershipCards.FirstOrDefault().Customer.Forename);
            }
            else if (sortOrder == WithdrawalSortOrder.CustomerNameDesc)
            {
                query = query.OrderByDescending(x => x.PartnerReward.MembershipCards.FirstOrDefault().Customer.Forename);
            }

            var result = await query.GetPaged(page, pageSize);
            return result;
        }


        public async Task<PagedResult<PartnerRewardWithdrawal>> GetCustomerWithdrawal(int statusId, int page, int pageSize,
            WithdrawalSortOrder sortOrder, DateTime startDate, DateTime endDate)
        {

            var query = _ctx.PartnerRewardWithdrawal.AsNoTracking()
                .Include(x => x.BankDetail)
                .ThenInclude(y => y.CustomerBankDetails)
                .ThenInclude(z => z.Customer)
                .Where(x => x.StatusId == statusId && x.PartnerRewardId == null && x.RequestedDate >= startDate.Date && x.RequestedDate <= endDate.Date);

            if (sortOrder == WithdrawalSortOrder.CustomerNameAsc)
            {
                query = query.OrderBy(x => x.PartnerReward.MembershipCards.FirstOrDefault().Customer.Forename);
            }
            else if (sortOrder == WithdrawalSortOrder.CustomerNameDesc)
            {
                query = query.OrderByDescending(x => x.PartnerReward.MembershipCards.FirstOrDefault().Customer.Forename);
            }
            var result = await query.GetPaged(page, pageSize);
            return result;
        }

        #endregion
    }
}
