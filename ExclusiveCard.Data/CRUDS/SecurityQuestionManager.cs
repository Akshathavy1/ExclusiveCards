using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class SecurityQuestionManager : ISecurityQuestionManager
    {
        #region Private Member
        private readonly ExclusiveContext _ctx;
        #endregion

        #region Constructor
        public SecurityQuestionManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }
        #endregion

        public async Task<SecurityQuestion> Add(SecurityQuestion securityQuestion)
        {
            DbSet<SecurityQuestion> securityQuestions = _ctx.Set<SecurityQuestion>();
            securityQuestions.Add(securityQuestion);
            await _ctx.SaveChangesAsync();

            return securityQuestion;
        }

        public async Task<SecurityQuestion> Update(SecurityQuestion securityQuestion)
        {
            DbSet<SecurityQuestion> securityQuestions = _ctx.Set<SecurityQuestion>();
            securityQuestions.Update(securityQuestion);
            await _ctx.SaveChangesAsync();

            return securityQuestion;
        }

        public async Task<List<SecurityQuestion>> GetAll()
        {
            return await _ctx.SecurityQuestion
                .Where(x => x.IsActive).ToListAsync();
        }
    }
}
