using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IPaymentNotificationService
    {
        #region Writes

        Task<Models.DTOs.PaymentNotification> Add(PaymentNotification paymentNotification);

        Task<Models.DTOs.PaymentNotification> Update(PaymentNotification paymentNotification);

        Task DeleteAsync(PaymentNotification paymentNotification);

        #endregion

        #region Reads

        Task<Models.DTOs.PaymentNotification> GetByCustomerProviderId(string customerProviderId, int paymentProviderId,
            string trntype = null);

        #endregion

    }
}
