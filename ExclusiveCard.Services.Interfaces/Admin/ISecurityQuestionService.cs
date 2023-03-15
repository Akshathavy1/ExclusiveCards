using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ISecurityQuestionService
    {
        #region Writes  

        Task<SecurityQuestion> Add(SecurityQuestion securityQuestion);
        Task<SecurityQuestion> Update(SecurityQuestion securityQuestion);

        #endregion

        #region Reads

        Task<List<SecurityQuestion>> GetAll();

        #endregion
    }
}
