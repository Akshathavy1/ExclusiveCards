using System;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class CustomerPaymentManager : ICustomerPaymentManager
    {
        private readonly ExclusiveContext _ctx;

        public CustomerPaymentManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<CustomerPayment> Add(CustomerPayment customerPayment)
        {
            DbSet<CustomerPayment> customerPayments = _ctx.Set<CustomerPayment>();
            if (!customerPayments.Any(x => x.PaymentProviderRef == customerPayment.PaymentProviderRef))
            {
                customerPayments.Add(customerPayment);
                await _ctx.SaveChangesAsync();
            }
            else
            {
                return null;
            }
            return customerPayment;
        }

        public async Task<CustomerPayment> Update(CustomerPayment customerPayment)
        {
            DbSet<CustomerPayment> customerPayments = _ctx.Set<CustomerPayment>();
            customerPayments.Update(customerPayment);
            await _ctx.SaveChangesAsync();
            return customerPayment;
        }

        public async Task DeleteAsync(CustomerPayment customerPayment)
        {
            DbSet<CustomerPayment> customerPayments = _ctx.Set<CustomerPayment>();
            customerPayments.Remove(customerPayment);
            await _ctx.SaveChangesAsync();
        }

        public async Task<CustomerPayment> GetByPaymentProviderRef(string paymentProviderRef)
        {
            return await _ctx.CustomerPayment.FirstOrDefaultAsync(x => x.PaymentProviderRef == paymentProviderRef);
        }

        public async Task<CustomerPayment> GetByCustomerPaymentProviderId(string customerPaymentProviderId)
        {
            return await _ctx.CustomerPayment.Include(x => x.PaymentNotification)
                .FirstOrDefaultAsync(x => x.PaymentNotification.CustomerPaymentProviderId == customerPaymentProviderId);
        }

        public async Task<List<CustomerPayment>> GetAll()
        {
            return await _ctx.CustomerPayment.ToListAsync();
        }

        public async Task<CustomerPayment> GetByCustomerPaymentDateAmount(int customerId, DateTime paymentDate,
            decimal amount)
        {
            return await _ctx.CustomerPayment.FirstOrDefaultAsync(x =>
                x.CustomerId == customerId && x.Amount == amount && x.PaymentDate == paymentDate);
        }
    }
}
