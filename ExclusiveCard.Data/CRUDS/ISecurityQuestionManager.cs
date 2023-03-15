using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ISecurityQuestionManager
    {
        Task<SecurityQuestion> Add(SecurityQuestion securityQuestion);
        Task<SecurityQuestion> Update(SecurityQuestion securityQuestion);
        Task<List<SecurityQuestion>> GetAll();
    }
}