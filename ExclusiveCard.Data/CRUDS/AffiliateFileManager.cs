using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class AffiliateFileManager : IAffiliateFileManager
    {
        private readonly ExclusiveContext _ctx;

        public AffiliateFileManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<AffiliateFile> Add(AffiliateFile affiliateFile)
        {
            DbSet<AffiliateFile> affiliateFiles = _ctx.Set<AffiliateFile>();
            affiliateFiles.Add(affiliateFile);
            await _ctx.SaveChangesAsync();
            return affiliateFile;
        }

        public async Task<AffiliateFile> Update(AffiliateFile affiliateFile)
        {
            DbSet<AffiliateFile> affiliateFiles = _ctx.Set<AffiliateFile>();
            affiliateFiles.Update(affiliateFile);
            await _ctx.SaveChangesAsync();
            return affiliateFile;
        }

        public async Task<AffiliateFile> Get(int id)
        {
            return await _ctx.AffiliateFile.FirstOrDefaultAsync(x => x.Id == id);
        }

        
        public async Task<List<AffiliateFile>> GetByAffiliateAsync(int id)
        {
            return await _ctx.AffiliateFile.Where(x => x.AffiliateId == id).ToListAsync();
        }

        public async Task<List<AffiliateFile>> GetAll()
        {
            return await _ctx.AffiliateFile.ToListAsync();
        }
    }
}
