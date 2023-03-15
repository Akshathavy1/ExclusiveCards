using ExclusiveCard.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ST = ExclusiveCard.Data.StagingModels;

namespace ExclusiveCard.Data.CRUDS
{
    public class StagingCustomerRegistrationManager : IStagingCustomerRegistrationManager
    {
        private readonly ExclusiveContext _ctx;

        public StagingCustomerRegistrationManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ST.CustomerRegistration> AddAsync(ST.CustomerRegistration customerSerializeData)
        {
            DbSet<ST.CustomerRegistration> customerSerialize = _ctx.Set<ST.CustomerRegistration>();
            var entry = _ctx.Entry(customerSerializeData);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Set<ST.CustomerRegistration>().Attach(customerSerializeData);
            }
            customerSerialize.Add(customerSerializeData);
            await _ctx.SaveChangesAsync();
            entry.State = EntityState.Detached;

            return customerSerializeData;
        }

        public async Task<ST.CustomerRegistration> UpdateAsync(ST.CustomerRegistration customerSerializeData)
        {
            DbSet<ST.CustomerRegistration> customerSerialize = _ctx.Set<ST.CustomerRegistration>();
            var entry = _ctx.Entry(customerSerializeData);
            if (entry.State == EntityState.Detached)
            {
                _ctx.Set<ST.CustomerRegistration>().Attach(customerSerializeData);
            }
            customerSerialize.Update(customerSerializeData);
            await _ctx.SaveChangesAsync();
            entry.State = EntityState.Detached;

            return customerSerializeData;
        }

        public async Task<ST.CustomerRegistration> GetByCustomerPaymentIdAsync(Guid customerPaymentId, int? statusId)
        {
            IQueryable<ST.CustomerRegistration> query =
                _ctx.StagingCustomerRegistration.AsNoTracking().Where(x => x.CustomerPaymentId == customerPaymentId);

            if (statusId.HasValue)
            {
                query = query.Where(x => x.StatusId == statusId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
