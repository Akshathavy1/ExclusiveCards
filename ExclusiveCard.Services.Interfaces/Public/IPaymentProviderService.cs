using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IPaymentProviderService
    {
        #region Writes  

        Task<PaymentProvider> Add(PaymentProvider paymentProvider);

        Task<PaymentProvider> Update(PaymentProvider paymentProvider);

        #endregion

        #region Reads

        Task<PaymentProvider> GetByName(string name);

        #endregion
    }
}
