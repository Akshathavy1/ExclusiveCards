using ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IEmailCustomFieldManager
    {
        Task AddAsync(EmailCustomField emailCustomField);

        Task UpdateAsync(EmailCustomField emailCustomField);

        Task<EmailCustomField> GetByIdAsync(int id);

        Task<EmailCustomField> GetBySendGridIdAsync(string sendGridId);

        Task<List<EmailCustomField>> GetAllAsync();
    }
}
