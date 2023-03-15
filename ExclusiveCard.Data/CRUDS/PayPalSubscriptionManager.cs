using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class PayPalSubscriptionManager : IPayPalSubscriptionManager
    {
        private readonly ExclusiveContext _ctx;

        public PayPalSubscriptionManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PayPalSubscription> Add(PayPalSubscription paypalSubscription)
        {
            DbSet<PayPalSubscription> paypalSubscriptions = _ctx.Set<PayPalSubscription>();
            paypalSubscriptions.Add(paypalSubscription);
            await _ctx.SaveChangesAsync();

            return paypalSubscription;
        }

        public async Task<PayPalSubscription> Update(PayPalSubscription paypalSubscription)
        {
            DbSet<PayPalSubscription> paypalSubscriptions = _ctx.Set<PayPalSubscription>();
            paypalSubscriptions.Update(paypalSubscription);
            await _ctx.SaveChangesAsync();

            return paypalSubscription;
        }

        public async Task DeleteAsync(PayPalSubscription paypalSubscription)
        {
            DbSet<PayPalSubscription> paypalSubscriptions = _ctx.Set<PayPalSubscription>();
            paypalSubscriptions.Update(paypalSubscription);
            await _ctx.SaveChangesAsync();
        }

        public async Task<PayPalSubscription> GetByPayPalId(string paypalId)
        {
            return await _ctx.PayPalSubscription.OrderByDescending(x => x.NextPaymentDate)
                .FirstOrDefaultAsync(x => x.PayPalId == paypalId);
        }

        public async Task<PayPalSubscription> GetByCustomerId(int customerId)
        {
            return await _ctx.PayPalSubscription.OrderByDescending(x => x.NextPaymentDate)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }
    }
}
