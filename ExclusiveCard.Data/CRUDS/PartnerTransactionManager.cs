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
    public class PartnerTransactionManager : IPartnerTransactionManager
    {
        #region Private Members and Constructor

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public PartnerTransactionManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Writes

        public async Task UpdatePartnerTransactions(int partnerRewardId, int fileId, int paymentStatusId)
        {
            try
            {
                var cashbackTransaction = await _ctx.CashbackTransaction
                    .Where(x => x.MembershipCard.PartnerRewardId == partnerRewardId && x.FileId == fileId).ToListAsync();
                if (cashbackTransaction?.Count > 0)
                {
                    foreach (var item in cashbackTransaction)
                    {
                        item.PaymentStatusId = paymentStatusId;
                    }

                    DbSet<CashbackTransaction> cashbackTransactionSet = _ctx.Set<CashbackTransaction>();
                    cashbackTransactionSet.UpdateRange(cashbackTransaction);
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        #endregion

        #region Reads

        public async Task<List<TamDataModel>> GetTransactionReport(int partnerId)
        {
            List<TamDataModel> data = new List<TamDataModel>();
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetTransactionReport]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@PartnerId";
                para1.DbType = DbType.Int32;
                para1.Value = partnerId;

                cmd.Parameters.Add(para1);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Add(new TamDataModel
                            {
                                FileId = Convert.ToInt32(reader["FileId"].ToString()),
                                TransType = reader["TransType"].ToString(),
                                UniqueReference = reader["UniqueReference"].ToString(),
                                FundType = reader["FundType"].ToString(),
                                Title = reader["Title"].ToString(),
                                Forename = reader["Forename"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                NINumber = reader["NINumber"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                IntroducerCode = string.Empty,
                                ProcessState = string.Empty
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

            return data;
        }

        public async Task<PagedResult<Files>> GetTransactionsAsync(int statusId, int partnerId, int page, int pageSize, TransactionSortOrder sortOrder)
        {
            try
            {
                IQueryable<Files> fileQuery =
                    _ctx.Files.Include(x => x.PaymentStatus)
                        .Where(x => x.PartnerId == partnerId && x.Status.Name == Data.Constants.Status.Processed &&
                                    x.Status.Type == StatusType.FileStatus
                                    && x.PaymentStatusId == statusId);

                if (sortOrder.Equals(TransactionSortOrder.DateAsc))
                {
                    fileQuery = fileQuery.OrderBy(x => x.CreatedDate.Date).ThenBy(x => x.CreatedDate.TimeOfDay);
                }
                else if (sortOrder.Equals(TransactionSortOrder.DateDesc))
                {
                    fileQuery = fileQuery.OrderByDescending(x => x.CreatedDate.Date)
                        .ThenBy(x => x.CreatedDate.TimeOfDay);
                }
                else if (sortOrder.Equals(TransactionSortOrder.FileNameAsync))
                {
                    fileQuery = fileQuery.OrderBy(x => x.Name);
                }
                else if (sortOrder.Equals(TransactionSortOrder.FileNameDesc))
                {
                    fileQuery = fileQuery.OrderByDescending(x => x.Name);
                }

                return await fileQuery.GetPaged(page, pageSize);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public async Task<PagedResult<CustomerWithdrawalDataModel>> GetPagedCustomerRewardWithdrawalsAsync(
            DateTime fromDate, DateTime toDate, int page, int pageSize)
        {
            PagedResult<CustomerWithdrawalDataModel> data = new PagedResult<CustomerWithdrawalDataModel>
            {
                PageCount = pageSize,
                CurrentPage = page,
                Results = new List<CustomerWithdrawalDataModel>()
            };
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetPagedCustomerPartnerWithdrawals]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                //para1.ParameterName = "@PartnerId";
                //para1.DbType = DbType.Int32;
                //para1.Value = partnerId;
                var para2 = cmd.CreateParameter();
                para2.ParameterName = "@FromDate";
                para2.DbType = DbType.DateTime;
                para2.Value = fromDate;
                var para3 = cmd.CreateParameter();
                para3.ParameterName = "@ToDate";
                para3.DbType = DbType.DateTime;
                para3.Value = toDate;
                var para4 = cmd.CreateParameter();
                para4.ParameterName = "@PageNumber";
                para4.DbType = DbType.Int32;
                para4.Value = page;
                var para5 = cmd.CreateParameter();
                para5.ParameterName = "@PageSize";
                para5.DbType = DbType.Int32;
                para5.Value = pageSize;

                // cmd.Parameters.Add(para1);
                cmd.Parameters.Add(para2);
                cmd.Parameters.Add(para3);
                cmd.Parameters.Add(para4);
                cmd.Parameters.Add(para5);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Results.Add(new CustomerWithdrawalDataModel()
                            {
                                TotalRecord = Convert.ToInt64(reader["TotalRecord"].ToString()),
                                RowNumber = Convert.ToInt64(reader["RowNumber"].ToString()),
                                ContactName = reader["ContactName"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"].ToString()),
                                DueDate = Convert.ToDateTime(reader["DueDate"].ToString()),
                                Total = Convert.ToDecimal(reader["Total"].ToString()),
                                Description = reader["Description"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
                                UnitAmount = Convert.ToDecimal(reader["UnitAmount"].ToString()),
                                AccountCode = Convert.ToInt32(reader["AccountCode"].ToString()),
                                TaxType = Convert.ToInt32(reader["TaxType"].ToString()),
                                TaxAmount = Convert.ToInt32(reader["TaxAmount"].ToString()),
                                Currency = reader["Currency"].ToString()
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

        public async Task<List<CustomerWithdrawalDataModel>> GetCustomerRewardWithdrawalsAsync(int partnerId,
            DateTime fromDate, DateTime toDate)
        {
            List<CustomerWithdrawalDataModel> data = new List<CustomerWithdrawalDataModel>();
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetCustomerPartnerWithdrawals]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@PartnerId";
                para1.DbType = DbType.Int32;
                para1.Value = partnerId;
                var para2 = cmd.CreateParameter();
                para2.ParameterName = "@FromDate";
                para2.DbType = DbType.DateTime;
                para2.Value = fromDate;
                var para3 = cmd.CreateParameter();
                para3.ParameterName = "@ToDate";
                para3.DbType = DbType.DateTime;
                para3.Value = toDate;

                cmd.Parameters.Add(para1);
                cmd.Parameters.Add(para2);
                cmd.Parameters.Add(para3);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Add(new CustomerWithdrawalDataModel()
                            {
                                ContactName = reader["ContactName"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"].ToString()),
                                DueDate = Convert.ToDateTime(reader["DueDate"].ToString()),
                                Total = Convert.ToDecimal(reader["Total"].ToString()),
                                Description = reader["Description"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
                                UnitAmount = Convert.ToDecimal(reader["UnitAmount"].ToString()),
                                AccountCode = Convert.ToInt32(reader["AccountCode"].ToString()),
                                TaxType = Convert.ToInt32(reader["TaxType"].ToString()),
                                TaxAmount = Convert.ToInt32(reader["TaxAmount"].ToString()),
                                Currency = reader["Currency"].ToString()
                            });
                        }
                    }
                }

                _ctx.Database.CloseConnection();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return data;
        }

        #endregion
    }
}
