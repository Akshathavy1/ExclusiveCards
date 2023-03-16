using ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Interfaces.Public    
{
    public interface IMerchantService
    {
        #region Reads

        Merchant Get(int id, bool includeBranch = false, bool includeBranchContact = false, bool includeImage = false, bool includeSocialMedia = false);

        #endregion
        #region 
        List<Merchant> GetAllMerchants();
        
        #endregion
    }
}
