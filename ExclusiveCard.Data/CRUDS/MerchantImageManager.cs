using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class MerchantImageManager : IMerchantImageManager
    {
        private readonly ExclusiveContext _ctx;

        public MerchantImageManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<MerchantImage> Add(MerchantImage merchantImage)
        {
            DbSet<MerchantImage> merchantImages = _ctx.Set<MerchantImage>();
            merchantImages.Add(merchantImage);
            await _ctx.SaveChangesAsync();
            return merchantImage;
        }

        public async Task<MerchantImage> Update(MerchantImage merchantImage)
        {
            DbSet<MerchantImage> merchantImages = _ctx.Set<MerchantImage>();
            merchantImages.Update(merchantImage);
            await _ctx.SaveChangesAsync();
            return merchantImage;
        }

        public async Task<int> Delete(int merchantId, short? displayOrder)
        {
            IQueryable<MerchantImage> merchantImageQuery = _ctx.MerchantImage
               .Where(x => x.MerchantId == merchantId);
            if(displayOrder > 0)
            {
                merchantImageQuery = merchantImageQuery.Where(x => x.DisplayOrder == displayOrder);
            }
            var merchantImages = await merchantImageQuery.ToListAsync();

            DbSet<MerchantImage> merchantImagesSet = _ctx.Set<MerchantImage>();
            merchantImagesSet.RemoveRange(merchantImages);
            await _ctx.SaveChangesAsync();
            return merchantId;
        }

        public async Task<int> DeleteByMerchantIdAndType(int merchantId, int type)
        {
            IQueryable<MerchantImage> merchantImageQuery = _ctx.MerchantImage
                .Where(x => x.MerchantId == merchantId);
            if (type > 0)
            {
                merchantImageQuery = merchantImageQuery.Where(x => x.ImageType == type);
            }
            var merchantImages = await merchantImageQuery.ToListAsync();
            DbSet<MerchantImage> merchantImagesSet = _ctx.Set<MerchantImage>();
            merchantImagesSet.RemoveRange(merchantImages);
            await _ctx.SaveChangesAsync();
            return merchantId;
        }

        public async Task<string> DeleteByMerchantImagePath(string path)
        {
            IQueryable<MerchantImage> merchantImageQuery = _ctx.MerchantImage
                .Where(x => x.ImagePath == path);

            var merchantImages = await merchantImageQuery.ToListAsync();
            DbSet<MerchantImage> merchantImagesSet = _ctx.Set<MerchantImage>();
            merchantImagesSet.RemoveRange(merchantImages);
            await _ctx.SaveChangesAsync();
            return path;
        }

        public MerchantImage Get(int merchantId)
        {
            MerchantImage merchantImage = new MerchantImage();
            if (_ctx.MerchantImage.Any(x => x.MerchantId == merchantId))
            {
               return _ctx.MerchantImage.OrderByDescending(x => x.DisplayOrder).FirstOrDefault(x => x.MerchantId == merchantId);
            }
            return merchantImage;
        }

        public async Task<List<MerchantImage>> GetAll(int merchantId, string imageSize, short? displayOrder)
        {
            IQueryable<MerchantImage> merchantImageQuery = _ctx.MerchantImage
                    .Where(x => x.MerchantId == merchantId);
            if (!string.IsNullOrEmpty(imageSize))
            {
                merchantImageQuery = merchantImageQuery.Where(x => x.ImagePath.Contains(imageSize));
            }
            if(displayOrder > 0)
            {
                merchantImageQuery = merchantImageQuery.Where(x => x.DisplayOrder == displayOrder);
            }
            return await merchantImageQuery.OrderBy(x => x.DisplayOrder).ToListAsync();
        }

        public async Task<MerchantImage> GetByIdAsync(int id)
        {
            MerchantImage merchantImage = new MerchantImage();
            if (_ctx.MerchantImage.Any(x => x.Id == id))
            {
                return await _ctx.MerchantImage.OrderByDescending(x => x.DisplayOrder).FirstOrDefaultAsync(x => x.Id == id);
            }
            return merchantImage;
        }
    }
}
