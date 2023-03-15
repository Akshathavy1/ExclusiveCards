using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IPartnerService
    {
        Task<string> LoginAsync(string userName, string password, string audience);

        Task LogoutAsync();

        Task<string> CustomerSignInAsync(string customerName, string token);

        bool ValidateLoginToken(string token, string audience);

        Task<Tuple<string, MembershipCard>> ValidateCustomerAndPartnerAsync(string customerName, string token);
    }
}
