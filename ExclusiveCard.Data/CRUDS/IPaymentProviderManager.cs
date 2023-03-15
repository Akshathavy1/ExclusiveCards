using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPaymentProviderManager
    {
        Task<PaymentProvider> Add(PaymentProvider paymentProvider);
        Task<PaymentProvider> Update(PaymentProvider paymentProvider);
        Task<PaymentProvider> GetByName(string name);
    }
}