using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMembershipPlanPaymentProviderManager
    {
        Task<MembershipPlanPaymentProvider> AddAsync(MembershipPlanPaymentProvider membershipPlanPaymentProvider);
        Task<MembershipPlanPaymentProvider> UpdateAsync(MembershipPlanPaymentProvider membershipPlanPaymentProvider);
        Task<List<MembershipPlanPaymentProvider>> Get(int membershipPlanId);
    }
}