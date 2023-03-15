using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Services.Admin
{
    internal class OfferBranchServices : IOfferBranchServices
    {
        private readonly ExclusiveContext _ctx;

        public OfferBranchServices(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<int>> GetofferBranch(int offerId)
        {
            IQueryable<OfferMerchantBranch> query = _ctx.OfferMerchantBranch;

            var branches = await query.Where(x => x.OfferId == offerId).ToListAsync();
            var merchantBranchIds = branches.Select(x => x.MerchantBranchId).ToList();
            return merchantBranchIds;
        }
    }
}