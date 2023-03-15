using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using System.Linq;

namespace ExclusiveCard.Data.CRUDS
{
    public class WebsiteSocialMediaManager : IWebsiteSocialMediaManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public WebsiteSocialMediaManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<WebsiteSocialMediaLink>> GetAllAsync()
        {
            return await _ctx.WebsiteSocialMediaLink.ToListAsync();
        }

        public async Task<List<WebsiteSocialMediaLink>> GetSocialMediaLinks(int id)
        {
            return await _ctx.WebsiteSocialMediaLink.Where(x => x.WhiteLabelSettingsId == id).AsNoTracking().ToListAsync();
        }

        #endregion
    }
}
