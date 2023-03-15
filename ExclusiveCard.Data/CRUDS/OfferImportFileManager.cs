using ExclusiveCard.Data.Context;
using ST = ExclusiveCard.Data.StagingModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ExclusiveCard.Data.CRUDS
{
    public class OfferImportFileManager : IOfferImportFileManager
    {
        private readonly ExclusiveContext _ctx;

        public OfferImportFileManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ST.OfferImportFile> Add(ST.OfferImportFile offerImportFile)
        {
            DbSet<ST.OfferImportFile> offerImportFiles = _ctx.Set<ST.OfferImportFile>();
            offerImportFiles.Add(offerImportFile);
            await _ctx.SaveChangesAsync();

            return offerImportFile;
        }

        public async Task<ST.OfferImportFile> Update(ST.OfferImportFile offerImportFile)
        {
            DbSet<ST.OfferImportFile> offerImportFiles = _ctx.Set<ST.OfferImportFile>();
            offerImportFiles.Update(offerImportFile);
            await _ctx.SaveChangesAsync();

            return offerImportFile;
        }

        public ST.OfferImportFile Get(int affiliateId, int fileTypeId, int status)
        {
            return _ctx.StagingOfferImportFile.FirstOrDefault(x =>
                        x.AffiliateFile.AffiliateId == affiliateId && 
                        x.AffiliateFile.Id == fileTypeId &&
                        x.AffiliateFileId == fileTypeId && 
                        x.ImportStatus != status);
        }

        public async Task<ST.OfferImportFile> GetById(int id, int? status = 0)
        {
            IQueryable<ST.OfferImportFile> offerImportFileQuery = _ctx.StagingOfferImportFile;
            if (status > 0)
            {
                offerImportFileQuery = offerImportFileQuery
                    .Include(x => x.OfferImportAwins)
                    .Where(x => x.ImportStatus != status);
            }
            return await offerImportFileQuery.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ST.OfferImportFile>> GetAllAsync(int? importStatus)
        {
            IQueryable<ST.OfferImportFile> offerImportFileQuery = _ctx.StagingOfferImportFile;
            if (importStatus > 0)
            {
                offerImportFileQuery = offerImportFileQuery
                    .Include(x => x.AffiliateFile)
                    .Where(x => x.ImportStatus == importStatus);
            }
            return await offerImportFileQuery.ToListAsync();
        }
    }
}
