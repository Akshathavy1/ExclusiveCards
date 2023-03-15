using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class PaymentNotificationManager : IPaymentNotificationManager
    {
        private readonly ExclusiveContext _ctx;

        public PaymentNotificationManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PaymentNotification> Add(PaymentNotification paymentNotification)
        {
            DbSet<PaymentNotification> paymentProviders = _ctx.Set<PaymentNotification>();
            paymentProviders.Add(paymentNotification);
            await _ctx.SaveChangesAsync();

            return paymentNotification;
        }

        public async Task<PaymentNotification> Update(PaymentNotification paymentNotification)
        {
            DbSet<PaymentNotification> paymentProviders = _ctx.Set<PaymentNotification>();
            paymentProviders.Update(paymentNotification);
            await _ctx.SaveChangesAsync();

            return paymentNotification;
        }

        public async Task DeleteAsync(PaymentNotification paymentNotification)
        {
            DbSet<PaymentNotification> paymentProviders = _ctx.Set<PaymentNotification>();
            paymentProviders.Remove(paymentNotification);
            await _ctx.SaveChangesAsync();
        }

        public async Task<PaymentNotification> GetByCustomerProviderId(string customerPaymentProviderId, int paymentProviderId, string trntype = null)
        {
            if (!string.IsNullOrEmpty(trntype))
            {
                return await _ctx.PaymentNotification
                    .FirstOrDefaultAsync(x => x.CustomerPaymentProviderId == customerPaymentProviderId
                                              && x.PaymentProviderId == paymentProviderId &&
                                              x.TransactionType == trntype);
            }
            return await _ctx.PaymentNotification
                .FirstOrDefaultAsync(x => x.CustomerPaymentProviderId == customerPaymentProviderId
                                            && x.PaymentProviderId == paymentProviderId);
        }
    }
}
