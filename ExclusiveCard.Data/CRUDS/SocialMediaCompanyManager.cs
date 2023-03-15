using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public class SocialMediaCompanyManager : ISocialMediaCompanyManager
    {
        private readonly ExclusiveContext _ctx;

        public SocialMediaCompanyManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<SocialMediaCompany> Add(SocialMediaCompany company)
        {
            DbSet<SocialMediaCompany> mediaCompanies = _ctx.Set<SocialMediaCompany>();
            mediaCompanies.Add(company);
            await _ctx.SaveChangesAsync();
            return company;
        }

        public async Task<List<SocialMediaCompany>> GetAll()
        {
            return await _ctx.SocialMediaCompany
                .Where(x => x.IsEnabled).ToListAsync();
        }
    }
}
