using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    [Obsolete("DO NOT write any new code or add references to this, use Exclusivecard.Services.Interfaces.IMembershipService instead")]
    public interface IOLD_MembershipRegistrationCodeService
    {
        #region Writes

        Task<MembershipRegistrationCode> Add(MembershipRegistrationCode membershipRegistrationCode);
        Task<MembershipRegistrationCode> Update(MembershipRegistrationCode membershipRegistrationCode);

        #endregion

        #region Reads

        Task<MembershipRegistrationCode> Get(string code);

        Task<List<MembershipRegistrationCode>> GetAllAsync();

        #endregion
    }
}
