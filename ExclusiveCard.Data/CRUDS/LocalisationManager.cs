using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class LocalisationManager : ILocalisationManager
    {
        private readonly ExclusiveContext _ctx;

        public LocalisationManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Localisation> Add(Localisation locale)
        {
            DbSet<Localisation> local = _ctx.Set<Localisation>();
            local.Add(locale);
            await _ctx.SaveChangesAsync();
            return locale;
        }

        public async Task<Localisation> Update(Localisation locale)
        {
            DbSet<Localisation> local = _ctx.Set<Localisation>();
            local.Update(locale);
            await _ctx.SaveChangesAsync();

            return locale;
        }

        public Localisation Get(int id)
        {
            return _ctx.Localisation.FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<Localisation>> GetAll(string localisationCode)
        {
            return await _ctx.Localisation.Where(x => x.LocalisationCode == localisationCode).ToListAsync();
        }

        public string GetByContext(string context, string localisationCode)
        {
            return _ctx.Localisation.FirstOrDefault(x => x.LocalisationCode == localisationCode && x.Context == context)?.LocalisedText;
        }
    }
}
