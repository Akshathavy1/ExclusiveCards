using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using System;
using System.Data.SqlClient;
using NLog;
using ExclusiveCard.Data.Repositories;

namespace ExclusiveCard.Data.CRUDS
{
    public class MerchantManager : IMerchantManager
    {
        private readonly IRepository<Merchant> _merchantRepo;
        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public MerchantManager(IRepository<Merchant> merchantRepo,ExclusiveContext ctx)
        {
            _merchantRepo = merchantRepo;
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<Merchant> Add(Merchant merchant)
        {
            try
            {
                _merchantRepo.Create(merchant);
                _merchantRepo.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            await Task.CompletedTask;

            return merchant;
        }

        public async Task<Merchant> Update(Merchant merchant)
        {
            try
            {
                _merchantRepo.Update(merchant);
                _merchantRepo.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            await Task.CompletedTask;

            return merchant;
        }

        public Merchant Get(int id, bool includeBranch, bool includeBranchContact, bool includeImage, bool includeSocialMedia)
        {
            Merchant merchant = null;
            try
            {
                IQueryable<Merchant> merchantQuery = _ctx.Merchant;

                if (includeBranch && !includeBranchContact)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantBranches);
                }
                if (includeBranch && includeBranchContact)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantBranches).ThenInclude(c => c.ContactDetail);
                }
                if (includeImage)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantImages);
                }
                if (includeSocialMedia)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantSocialMediaLinks);
                }
                merchant = merchantQuery.AsNoTracking().FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            
            }
            return merchant;
        }

        public Merchant GetByName(string name)
        {
            try
            {
                return _ctx.Merchant.AsNoTracking().Include(x => x.MerchantSocialMediaLinks)
                    .FirstOrDefault(x => !x.IsDeleted && x.Name.Trim() == name.Trim());
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return null;
        }

        public async Task<List<Merchant>> GetAll(bool includeBranch, bool includeImage, bool includeSocialMedia)
        {
            List<Merchant> merchants = null;
            try
            {
                IQueryable<Merchant> merchantQuery = _ctx.Merchant;

                if (includeBranch)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantBranches);
                }
                if (includeImage)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantImages.Where(y => y.ImagePath.Contains("__2")));
                }
                if (includeSocialMedia)
                {
                    merchantQuery = merchantQuery.Include(x => x.MerchantSocialMediaLinks);
                }
                merchants = await merchantQuery.AsNoTracking()
                    .Where(x => !x.IsDeleted).OrderBy(x => x.Name.Trim()).ToListAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return merchants;
        }

        public async Task<PagedResult<Merchant>> GetPagedListAsync(string searchText, int page, int pageSize, MerchantsSortOrder sortOrder)
        {
            try
            {
                IQueryable<Merchant> merchantQuery = _ctx.Merchant.Where(x => !x.IsDeleted);
                if (!string.IsNullOrEmpty(searchText))
                {
                    merchantQuery = merchantQuery.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));
                }
                if (sortOrder.Equals(MerchantsSortOrder.MerchantNameAsc))
                {
                    merchantQuery = merchantQuery.OrderBy(x => x.Name);
                }
                if (sortOrder.Equals(MerchantsSortOrder.MerchantNameDesc))
                {
                    merchantQuery = merchantQuery.OrderByDescending(x => x.Name);
                }
                return await merchantQuery.AsNoTracking().GetPaged(page, pageSize);
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
            }
            catch (SqlException e)
            {
                _logger.Error(e);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return null;
        }
    }
}
