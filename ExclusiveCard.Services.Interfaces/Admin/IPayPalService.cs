using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IPayPalService
    {
        Task<int> ProcessIPN(string Ipn, string adminEmail);

    }
}
