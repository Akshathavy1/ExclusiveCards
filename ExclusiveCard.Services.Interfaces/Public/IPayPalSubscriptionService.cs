using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IPayPalSubscriptionService
    {
        #region Writes

        Task<Models.DTOs.PayPalSubscription> Add(Models.DTOs.PayPalSubscription payPalSubscription);

        Task<Models.DTOs.PayPalSubscription> Update(Models.DTOs.PayPalSubscription payPalSubscription);

        Task DeleteAsync(Models.DTOs.PayPalSubscription payPalSubscription);

        #endregion

        #region Reads

        Task<Models.DTOs.PayPalSubscription> GetByPayPalId(string paypalId);

        Task<Models.DTOs.PayPalSubscription> GetByCustomerId(int customerId);

        #endregion
    }
}
