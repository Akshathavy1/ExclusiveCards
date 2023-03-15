using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IBankDetailManager
    {
        BankDetail Create(BankDetail bankDetail);
        BankDetail Get(int bankDetailId);
        BankDetail Update(BankDetail bankDetail);

        CustomerBankDetail GetCustomerBankDetail(int customerId, int bankDetailId = 0);
        CustomerBankDetail CreateCustomerBankDetail(CustomerBankDetail customerBankDetail);
        CustomerBankDetail UpdateCustomerBankDetail(CustomerBankDetail customerBankDetail);
    }
}