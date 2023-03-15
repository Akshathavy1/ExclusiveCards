using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
   public interface ICustomerBankDetailService
    {
        #region Writes  

        Task<CustomerBankDetail> Add(CustomerBankDetail customerBankDetail);
        Task<CustomerBankDetail> Update(CustomerBankDetail customerBankDetail);
        Task<bool> Delete(CustomerBankDetail customerBankDetail);

        #endregion

        #region Reads

        CustomerBankDetail Get(int? customerId, int? bankDetailId);

        #endregion
    }
}
