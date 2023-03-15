using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ICustomerBankDetailManager
    {
        Task<CustomerBankDetail> Add(CustomerBankDetail customerBankDetail);
        Task<CustomerBankDetail> Update(CustomerBankDetail customerBankDetail);
        Task<bool> Delete(CustomerBankDetail detail);

        CustomerBankDetail Get(int? customerId, int? bankDetailId);
    }
}