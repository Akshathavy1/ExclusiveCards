using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class MerchantSocialMediaLinkManager : IMerchantSocialMediaLinkManager
    {
        private readonly ExclusiveContext _ctx;

        public MerchantSocialMediaLinkManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<MerchantSocialMediaLink> Add(MerchantSocialMediaLink medialLink)
        {
            DbSet<MerchantSocialMediaLink> merchantBranch = _ctx.Set<MerchantSocialMediaLink>();
            merchantBranch.Add(medialLink);
            await _ctx.SaveChangesAsync();
            return medialLink;
        }

        public async Task<List<MerchantSocialMediaLink>> AddListAsync(List<MerchantSocialMediaLink> merchantSocialMediaLinks)
        {
            DbSet<MerchantSocialMediaLink> merchantSocialMediaLinksSet = _ctx.Set<MerchantSocialMediaLink>();
            merchantSocialMediaLinksSet.AddRange(merchantSocialMediaLinks);
            await _ctx.SaveChangesAsync();
            return merchantSocialMediaLinks;
        }

        public async Task<MerchantSocialMediaLink> Update(MerchantSocialMediaLink medialLink)
        {
            DbSet<MerchantSocialMediaLink> mediaLinks = _ctx.Set<MerchantSocialMediaLink>();
            mediaLinks.Update(medialLink);
            await _ctx.SaveChangesAsync();
            return medialLink;
        }

        public async Task<List<MerchantSocialMediaLink>> GetAll(int merchantId)
        {
            return await _ctx.MerchantSocialMediaLink
                .Where(x => x.MerchantId == merchantId).ToListAsync();
        }

        public MerchantSocialMediaLink Get(int merchantId, int socialMediaId)
        {
            return _ctx.MerchantSocialMediaLink.AsNoTracking()
                .FirstOrDefault(x => x.MerchantId == merchantId && x.SocialMediaCompanyId == socialMediaId);
        }
    }
}
