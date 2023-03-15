using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
   public interface ICustomerSecurityQuestionService
    {
        #region Reads  

        Task<CustomerSecurityQuestion> Add(CustomerSecurityQuestion customerSecurityQuestion);
        Task<CustomerSecurityQuestion> Update(CustomerSecurityQuestion customerSecurityQuestion);

        #endregion

        CustomerSecurityQuestion Get(int customerId);
    }
}
