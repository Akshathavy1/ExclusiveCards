using System;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ICustomerService
    {
        #region reads

        PagedResult<CustomerSummary> GetAllPagedSearch(int page, int pageSize);

        PagedResult<CustomerSummary> GetPagedSearch(string userName, string foreName,
            string surName, string cardNumber, string postCode, int? cardStatus,string registrationCode, DateTime? dob, DateTime? dateOfIssue,
            int page, int pageSize);
        
        Customer GetDetails(int Id);

        #endregion

        #region write

        Task<Customer> Add(Customer customer);

        Task<Customer> Update(Customer customer);

        #endregion

    }
}
