using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface ICustomerManager
    {
        Customer Create(Customer customer);

        Customer Get(int customerId);

        Customer Get(string aspNetUserId);

        Customer UpdateCustomerSettings(Customer customer);

        int? FindCustomerId(string aspNetUserId);
    }
}
