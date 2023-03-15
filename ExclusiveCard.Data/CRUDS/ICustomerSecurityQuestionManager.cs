using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ICustomerSecurityQuestionManager
    {
        Task<CustomerSecurityQuestion> Add(CustomerSecurityQuestion customerSecurityQuestion);
        Task<CustomerSecurityQuestion> Update(CustomerSecurityQuestion customerSecurityQuestion);
        CustomerSecurityQuestion Get(int customerId);
    }
}