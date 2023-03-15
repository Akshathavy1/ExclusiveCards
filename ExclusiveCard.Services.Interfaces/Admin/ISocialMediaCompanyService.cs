using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ISocialMediaCompanyService
    {
        #region Writes

        Task<SocialMediaCompany> Add(SocialMediaCompany company);

        #endregion

        #region Reads

        Task<List<SocialMediaCompany>> GetAll();

        #endregion
    }
}