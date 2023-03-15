using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface IEmailManager
    {
        Task<string> SendEmailAsync(Email email);

        Task SendEmailAsync(string emailId, int emailTemplateType, object emailTemplateData, string subject = null,List<string> attachments = null);

        List<string> CreateFromEmailTemplate(int emailTemplateType, object emailTemplateData);
    }
}
