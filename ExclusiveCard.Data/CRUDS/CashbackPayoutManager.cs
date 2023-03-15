using System;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using NLog;
using System.Reflection;

namespace ExclusiveCard.Data.CRUDS
{
    public class CashbackPayoutManager : ICashbackPayoutManager
    {
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public CashbackPayoutManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<CashbackPayout> Add(CashbackPayout cashbackPayout)
        {
            DbSet<CashbackPayout> cashbackPayouts = _ctx.Set<CashbackPayout>();
            cashbackPayouts.Add(cashbackPayout);
            await _ctx.SaveChangesAsync();

            return cashbackPayout;
        }

        public async Task<CashbackPayout> Update(CashbackPayout cashbackPayout)
        {
            try
            {
                DbSet<CashbackPayout> cashbackPayouts = _ctx.Set<CashbackPayout>();
                cashbackPayouts.Update(cashbackPayout);
                await _ctx.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex);
            }
            return cashbackPayout;
        }

        public async Task<CashbackPayout> Get(int id)
        {
            return await _ctx.CashbackPayout.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CashbackPayout> GetByCustomerPartnerCurrency(int customerId, string currencyCode)
        {
            return await _ctx.CashbackPayout.LastOrDefaultAsync(x => x.CustomerId == customerId && x.CurrencyCode == currencyCode);
        }

        public async Task<CashbackPayout> GetByMembershipCard(int membershipCardId)
        {
            return await _ctx.CashbackPayout
                .Include(x => x.BankDetail)
                .FirstOrDefaultAsync(x =>
                x.Customer.MembershipCards.Any(y => y.Id == membershipCardId));
        }

        public async Task<List<CashbackPayout>> GetAll()
        {
            return await _ctx.CashbackPayout.ToListAsync();
        }

        public async Task<WithdrawalRequestModel> GetCashoutDataForRequest(string userId)
        {
            WithdrawalRequestModel data = new WithdrawalRequestModel();
            try
            {
                _ctx.Database.OpenConnection();
                DbCommand cmd = _ctx.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "[Exclusive].[SP_GetCashoutRequestData]";
                cmd.CommandType = CommandType.StoredProcedure;

                var para1 = cmd.CreateParameter();
                para1.ParameterName = "@UserId";
                para1.DbType = DbType.String;
                para1.Size = 40;
                para1.Value = userId;

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
                                BankDetailId = !string.IsNullOrEmpty(reader["BankDetailId"].ToString()) ? Convert.ToInt32(reader["BankDetailId"].ToString()) : 0,
                                PartnerRewardId = !string.IsNullOrEmpty(reader["PartnerRewardId"].ToString()) ? Convert.ToInt32(reader["PartnerRewardId"].ToString()) : 0,
                                AvailableFund = !string.IsNullOrEmpty(reader["AvailableFund"].ToString()) ? Convert.ToDecimal(reader["AvailableFund"].ToString()) : 0m,
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

        public async Task<PagedResult<CashbackPayout>> GetCashbackPaidoutData(int statusId, int page, int pageSize,
            WithdrawalSortOrder sortOrder)
        {
            IQueryable<CashbackPayout> query = _ctx.CashbackPayout
                .Include(x => x.Customer)
                .Include(x => x.Status)
                .Include(x => x.BankDetail);

            if (sortOrder == WithdrawalSortOrder.CustomerNameAsc)
            {
                query = query.OrderBy(x => x.Customer.Forename).ThenBy(x => x.Customer.Surname);
            }
            else if (sortOrder == WithdrawalSortOrder.CustomerNameDesc)
            {
                query = query.OrderByDescending(x => x.Customer.Forename).ThenByDescending(x => x.Customer.Surname);
            }

            var result = await query.Where(x => x.StatusId == statusId).GetPaged(page, pageSize);
            return result;
        }

        public decimal GetWithdrawnAmount(int customerId)
        {
            return _ctx.CashbackPayout.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.StatusId == (int)Cashback.PaidOut)
                .Sum(y => y.Amount);
        }


        public async Task<PagedResult<CashbackTransaction>> GetAllPagedFinancialReportSearch(int statusId, DateTime startDate, DateTime endDate, int page, int pageSize
             )
        {
            try
            {
                IQueryable<CashbackTransaction> query = _ctx.CashbackTransaction
                    .Include(x => x.MembershipCard)
                    .ThenInclude(x => x.MembershipPlan)
                    .ThenInclude(x => x.Partner)
                    .Include(x => x.MembershipCard.MembershipPlan.SiteClan);
                    

                var result = await query.Where(x => x.StatusId == statusId && x.AccountType == 'B' && x.DateReceived >= startDate.Date && x.DateReceived <= endDate.Date).GetPaged(page, pageSize);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        public List<SPFinancialReport> GetPagedFinancialReportSearch(int statusId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            List<SPFinancialReport> spCashBackTransaction = new List<SPFinancialReport>();


            DbCommand cmd = LoadStoredProc(_ctx, "[Exclusive].[SP_FinancialReport]");
            cmd = WithSqlParam(cmd, "@PageNumber", page);
            cmd = WithSqlParam(cmd, "@PageSize", pageSize);
            
            if (startDate.HasValue)
            {
                cmd = WithSqlParam(cmd, "@StartDate", startDate);
            }
            if (endDate.HasValue)
            {
                cmd = WithSqlParam(cmd, "@EndDate", endDate);
            }
            spCashBackTransaction = ExecuteStoredProc<SPFinancialReport>(cmd).ToList();


            return spCashBackTransaction;
        }
       
        public List<SPFinancialReport> GetAllPagedFinancialReport(DateTime startDate, DateTime endDate)
        {
            List<SPFinancialReport> spFinancialReport = new List<SPFinancialReport>();

            DbCommand cmd = LoadStoredProc(_ctx, "[Exclusive].[SP_FinancialReport]");
            cmd = WithSqlParam(cmd, "@StartDate", startDate);
            cmd = WithSqlParam(cmd, "@EndDate", endDate);
            spFinancialReport = ExecuteStoredProc<SPFinancialReport>(cmd).ToList();

            return spFinancialReport;
        }
        #region Store Procedure

        public static DbCommand LoadStoredProc(DbContext context, string storedProcName)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }

        public static DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            cmd.Parameters.Add(param);
            return cmd;
        }

        private static IList<T> MapToList<T>(DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static IList<T> ExecuteStoredProc<T>(DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return MapToList<T>(reader);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

       

        #endregion
    }
}
