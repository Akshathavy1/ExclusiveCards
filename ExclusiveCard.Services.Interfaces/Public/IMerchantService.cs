using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public    
{
    public interface IMerchantService
    {
        #region Reads

        Merchant Get(int id, bool includeBranch = false, bool includeBranchContact = false, bool includeImage = false, bool includeSocialMedia = false);

        #endregion
    }
}
