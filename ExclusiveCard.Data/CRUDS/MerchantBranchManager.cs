using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class MerchantBranchManager : IMerchantBranchManager
    {
        private readonly ExclusiveContext _ctx;

        public MerchantBranchManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<MerchantBranch> Add(MerchantBranch branch)
        {
            DbSet<MerchantBranch> merchantBranch = _ctx.Set<MerchantBranch>();
            merchantBranch.Add(branch);
            await _ctx.SaveChangesAsync();

            return branch;
        }

        public async Task<List<MerchantBranch>> GetAll(int merchantId, bool includeContacts)
        {
            IQueryable<MerchantBranch> query = _ctx.MerchantBranch;
            if (includeContacts)
            {
                query = query.Include(x => x.ContactDetail);
            }
            return await query.Where(x => !x.IsDeleted && x.MerchantId == merchantId).ToListAsync();
        }

        public async Task<MerchantBranch> Get(int id, bool includeContact, bool includeMerchant)
        {
            MerchantBranch branch;
            IQueryable<MerchantBranch> branchQuery = _ctx.MerchantBranch;
            if (includeContact)
            {
                branchQuery = branchQuery.Include(x => x.ContactDetail);
            }
            if (includeMerchant)
            {
                branchQuery = branchQuery.Include(x => x.Merchant);
            }
            branch = await branchQuery.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
            return branch;
        }

        public async Task<MerchantBranch> Update(MerchantBranch branch)
        {
                DbSet<MerchantBranch> merchantBranch = _ctx.Set<MerchantBranch>();
                merchantBranch.Update(branch);
                await _ctx.SaveChangesAsync();
                return branch;
        }

        public async Task<PagedResult<MerchantBranch>> GetBranchPagedList(int merchantId, int page, int pageSize)
        {
            return await _ctx.MerchantBranch
                .Include(x => x.ContactDetail)
                .Where(x => !x.IsDeleted && x.MerchantId == merchantId).OrderBy(x => x.DisplayOrder).GetPaged(page, pageSize);
        }
    }
}
