using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPayPalSubscriptionManager
    {
        Task<PayPalSubscription> Add(PayPalSubscription paypalSubscription);
        Task<PayPalSubscription> Update(PayPalSubscription paypalSubscription);
        Task DeleteAsync(PayPalSubscription paypalSubscription);
        Task<PayPalSubscription> GetByPayPalId(string paypalId);
        Task<PayPalSubscription> GetByCustomerId(int customerId);
    }
}