using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class CustomerSecurityQuestionManager : ICustomerSecurityQuestionManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public CustomerSecurityQuestionManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<CustomerSecurityQuestion> Add(CustomerSecurityQuestion customerSecurityQuestion)
        {
            DbSet<CustomerSecurityQuestion> securityQuestions = _ctx.Set<CustomerSecurityQuestion>();
            securityQuestions.Add(customerSecurityQuestion);
            await _ctx.SaveChangesAsync();
            return customerSecurityQuestion;
        }

        public async Task<CustomerSecurityQuestion> Update(CustomerSecurityQuestion customerSecurityQuestion)
        {
            DbSet<CustomerSecurityQuestion> customerSecurityQuestions = _ctx.Set<CustomerSecurityQuestion>();
            customerSecurityQuestions.Update(customerSecurityQuestion);
            await _ctx.SaveChangesAsync();

            return customerSecurityQuestion;
        }

        public CustomerSecurityQuestion Get(int customerId)
        {
            return _ctx.CustomerSecurityQuestion.AsNoTracking().FirstOrDefault(x => x.CustomerId == customerId);
        }
    }
}
