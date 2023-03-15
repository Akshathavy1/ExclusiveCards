using ExclusiveCard.Data.Context;
using ST = ExclusiveCard.Data.StagingModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferImportAwinManager : IOfferImportAwinManager
    {
        private readonly ExclusiveContext _ctx;

        public OfferImportAwinManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ST.OfferImportAwin> Add(ST.OfferImportAwin offerImport)
        {
            DbSet<ST.OfferImportAwin> offerImports = _ctx.Set<ST.OfferImportAwin>();
            offerImports.Add(offerImport);
            await _ctx.SaveChangesAsync();

            return offerImport;
        }

        public async Task<ST.OfferImportAwin> Update(ST.OfferImportAwin offerImport)
        {
            DbSet<ST.OfferImportAwin> offerImports = _ctx.Set<ST.OfferImportAwin>();
            offerImports.Update(offerImport);
            await _ctx.SaveChangesAsync();
            return offerImport;
        }

        public async Task Delete(List<ST.OfferImportAwin> awins)
        {
            //foreach (int id in ids)
            //{
            //ctx.StagingOfferImportAwin.RemoveRange(awins); //timing out
            DbSet<ST.OfferImportAwin> offerImportAwinSet = _ctx.Set<ST.OfferImportAwin>();
            offerImportAwinSet.RemoveRange(awins);
            await _ctx.SaveChangesAsync();
            //}
        }

        public async Task<List<ST.OfferImportAwin>> AddToAwinAsync(List<ST.OfferImportAwin> offerImportAwins)
        {
            DbSet<ST.OfferImportAwin> offerImportAwinSet = _ctx.Set<ST.OfferImportAwin>();
            offerImportAwinSet.AddRange(offerImportAwins);
            await _ctx.SaveChangesAsync();
            return offerImportAwins.Where(x => x.Id == 0).ToList();
        }

        public async Task<List<ST.OfferImportAwin>> GetAllAsync(int? importFileId)
        {
            IQueryable<ST.OfferImportAwin> offerImportAwinQuery = _ctx.StagingOfferImportAwin;
            if (importFileId > 0)
            {
                offerImportAwinQuery = offerImportAwinQuery.Where(x => x.OfferImportFileId == importFileId);
            }
            return await offerImportAwinQuery.ToListAsync();
        }

    }
}
