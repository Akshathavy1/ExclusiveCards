using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPaymentNotificationManager
    {
        Task<PaymentNotification> Add(PaymentNotification paymentNotification);
        Task<PaymentNotification> Update(PaymentNotification paymentNotification);
        Task DeleteAsync(PaymentNotification paymentNotification);

        Task<PaymentNotification> GetByCustomerProviderId(string customerPaymentProviderId, int paymentProviderId,
            string trntype = null);
    }
}