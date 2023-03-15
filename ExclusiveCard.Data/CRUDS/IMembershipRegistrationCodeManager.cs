using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMembershipRegistrationCodeManager
    {
        Task<MembershipRegistrationCode> Add(MembershipRegistrationCode membershipRegistrationCode);
        Task<MembershipRegistrationCode> Update(MembershipRegistrationCode membershipRegistrationCode);
        Task<MembershipRegistrationCode> GetAsync(string code);
        Task<List<MembershipRegistrationCode>> GetAllAsync();
    }
}