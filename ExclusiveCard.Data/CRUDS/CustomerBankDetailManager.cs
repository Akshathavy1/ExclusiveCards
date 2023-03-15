using System;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class CustomerBankDetailManager : ICustomerBankDetailManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public CustomerBankDetailManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        public async Task<CustomerBankDetail> Add(CustomerBankDetail customerBankDetail)
        {
            DbSet<CustomerBankDetail> customerBankDetails = _ctx.Set<CustomerBankDetail>();
            var entry = _ctx.Entry(customerBankDetail);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Set<CustomerBankDetail>().Attach(customerBankDetail);
            }
            customerBankDetails.Add(customerBankDetail);
            await _ctx.SaveChangesAsync();
            entry.State = EntityState.Detached;
            return customerBankDetail;
        }

        public async Task<CustomerBankDetail> Update(CustomerBankDetail customerBankDetail)
        {
            try
            {
                DbSet<CustomerBankDetail> customerBankDetails = _ctx.Set<CustomerBankDetail>();
                var entry = _ctx.Entry(customerBankDetail);
                if (entry.State == EntityState.Detached)
                {
                    _ctx.Set<CustomerBankDetail>().Attach(customerBankDetail);
                }

                customerBankDetails.Update(customerBankDetail);
                await _ctx.SaveChangesAsync();
                entry.State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return customerBankDetail;
        }

        public async Task<bool> Delete(CustomerBankDetail detail)
        {
            try
            {
                DbSet<CustomerBankDetail> customerBankDetails = _ctx.Set<CustomerBankDetail>();
                var data = _ctx.CustomerBankDetail.AsNoTracking().FirstOrDefault(x => x.CustomerId == detail.CustomerId && x.BankDetailsId == detail.BankDetailsId);
                if (data != null)
                {
                    var entry = _ctx.Entry(detail);
                    if (entry.State == EntityState.Detached)
                    {
                        _ctx.Set<CustomerBankDetail>().Attach(detail);
                    }
                    customerBankDetails.Remove(data);
                    await _ctx.SaveChangesAsync();
                    entry.State = EntityState.Detached;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public CustomerBankDetail Get(int? customerId, int? bankDetailId)
        {
            IQueryable<CustomerBankDetail> query = _ctx.CustomerBankDetail;
            if (customerId.HasValue && !bankDetailId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId.Value);
            }
            if (bankDetailId.HasValue && !customerId.HasValue)
            {
                query = query.Where(x => x.BankDetailsId == bankDetailId.Value);
            }

            if (customerId.HasValue && bankDetailId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId.Value && x.BankDetailsId == bankDetailId.Value);
            }

            return query.AsNoTracking().FirstOrDefault(x => x.IsActive && !x.IsDeleted);
        }
    }
}
