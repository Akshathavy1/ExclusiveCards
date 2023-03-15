using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Providers.Email
{
    public interface IEmailProvider
    {
        Task<string> SendEmailAsync(dto.Email email);

    }
}
