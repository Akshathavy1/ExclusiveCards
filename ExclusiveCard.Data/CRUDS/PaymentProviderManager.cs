using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class PaymentProviderManager : IPaymentProviderManager
    {
        private readonly ExclusiveContext _ctx;

        public PaymentProviderManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PaymentProvider> Add(PaymentProvider paymentProvider)
        {
            DbSet<PaymentProvider> paymentProviders = _ctx.Set<PaymentProvider>();
            paymentProviders.Add(paymentProvider);
            await _ctx.SaveChangesAsync();

            return paymentProvider;
        }

        public async Task<PaymentProvider> Update(PaymentProvider paymentProvider)
        {
            DbSet<PaymentProvider> paymentProviders = _ctx.Set<PaymentProvider>();
            paymentProviders.Update(paymentProvider);
            await _ctx.SaveChangesAsync();

            return paymentProvider;
        }

        public async Task<PaymentProvider> GetByName(string name)
        {
            return await _ctx.PaymentProvider.FirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }
    }
}
