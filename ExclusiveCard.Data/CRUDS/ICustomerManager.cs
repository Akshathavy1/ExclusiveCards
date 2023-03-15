using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ICustomerManager
    {
        Task<Customer> Add(Customer customer);
        Task<LoginUserToken> AddLoginToken(LoginUserToken loginUserToken);
        LoginUserToken GetUserTokenByToken(string token);
        LoginUserToken GetUserTokenByUserId(string aspNetUserId);
        LoginUserToken GetUserTokenByTokenValue(Guid tokenValue);
        Task<Customer> Update(Customer customer);
        Task<Customer> DeleteAsync(Customer customer);
        Task<Customer> GetAsync(string aspNetUserId);
        Customer GetDetails(int id);
        Customer GetCustomerByUserName(string userName);

        List<SPCustomerSearch> GetPagedSearch(string userName, string foreName, string surName,
            string cardNumber, string postCode, int? cardStatus,string registrationCode, DateTime? dob, DateTime? dateOfIssue, int page,
            int pageSize);

        List<SPCustomerSearch> GetAllPagedSearch(int page, int pageSize);
        Task<Customer> GetCustomerAsync(int id);

        Task<List<Customer>> GetCustomersToBeAddedToSendGridAsync();

        Task<List<Customer>> GetCustomersToBeRemovedFromSendGridAsync();
    }
}