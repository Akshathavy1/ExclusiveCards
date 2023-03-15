using System;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ICustomerService
    {
        #region Writes  

        // REPLACING Add with a proper service
        //Task<Customer> Add(Customer customer);

        Task<Customer> Update(Customer customer);
        Task<LoginUserToken> AddLoginToken(LoginUserToken userToken);
        Task<Customer> DeleteAsync(Customer customer);
        
        #endregion

        #region Reads

        Task<Customer> Get(string aspNetUserId);
        LoginUserToken GetUserTokenByTokenValue(Guid tokenValue);
        Customer GetDetails(int id);

        Customer GetCustomerByUserName(string userName);

        #endregion
    }
}
