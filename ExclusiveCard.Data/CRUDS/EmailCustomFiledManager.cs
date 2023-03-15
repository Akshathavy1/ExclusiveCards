using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class EmailCustomFiledManager : IEmailCustomFieldManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public EmailCustomFiledManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task AddAsync(EmailCustomField emailCustomField)
        {
            DbSet<EmailCustomField> emailCustomFields = _ctx.Set<EmailCustomField>();
            emailCustomFields.Add(emailCustomField);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailCustomField emailCustomField)
        {
            DbSet<EmailCustomField> emailCustomFields = _ctx.Set<EmailCustomField>();
            emailCustomFields.Update(emailCustomField);
            await _ctx.SaveChangesAsync();
        }

        public async Task<EmailCustomField> GetByIdAsync(int id)
        {
            return await _ctx.EmailCustomField.AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EmailCustomField> GetBySendGridIdAsync(string sendGridId)
        {
            return await _ctx.EmailCustomField.AsNoTracking().
                FirstOrDefaultAsync(x => x.SendGridId == sendGridId);
        }

        public async Task<List<EmailCustomField>> GetAllAsync()
        {
            return await
                _ctx.EmailCustomField.AsNoTracking().ToListAsync();
        }


    }
}
