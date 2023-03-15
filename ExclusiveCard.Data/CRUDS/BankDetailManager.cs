using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class BankDetailManager : IBankDetailManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public BankDetailManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<BankDetail> Add(BankDetail bankDetail)
        {
            DbSet<BankDetail> bankDetails = _ctx.Set<BankDetail>();
            bankDetails.Add(bankDetail);
            await _ctx.SaveChangesAsync();
            return bankDetail;
        }

        public async Task<BankDetail> Update(BankDetail bankDetail)
        {
            DbSet<BankDetail> bankDetails = _ctx.Set<BankDetail>();
            bankDetails.Update(bankDetail);
            await _ctx.SaveChangesAsync();

            return bankDetail;
        }

        public async Task<BankDetail> Get(int id)
        {
            return await _ctx.BankDetail.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<List<BankDetail>> GetAll()
        {
            return await _ctx.BankDetail.Where(x => !x.IsDeleted).ToListAsync();
        }
    }
}
