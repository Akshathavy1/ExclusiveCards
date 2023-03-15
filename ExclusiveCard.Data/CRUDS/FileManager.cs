using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using NLog;

namespace ExclusiveCard.Data.CRUDS
{
    public class FileManager : IFileManager
    {
        #region Private Members and Constructor

        private readonly ExclusiveContext _ctx;
        private readonly ILogger _logger;

        public FileManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Writes

        public async Task<Files> AddAsync(Files file)
        {
            try
            {
                DbSet<Files> files = _ctx.Set<Files>();
                var entry = _ctx.Entry(file);
                if (entry.State == EntityState.Detached)
                {
                    _ctx.Set<Files>().Attach(file);
                }
                files.Add(file);
                await _ctx.SaveChangesAsync();
                entry.State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return file;
        }

        public async Task<Files> UpdateAsync(Files file)
        {
            try
            {
                DbSet<Files> files = _ctx.Set<Files>();
                var entry = _ctx.Entry(file);
                if (entry.State == EntityState.Detached)
                {
                    _ctx.Set<Files>().Attach(file);
                }
                files.Update(file);
                await _ctx.SaveChangesAsync();
                _ctx.Entry(file).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return file;
        }

        #endregion

        #region Reads

        public async Task<Files> GetByIdAsync(int id)
        {
            return await _ctx.Files.Include(x => x.Status).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Files> GetByNameAsync(string name)
        {
            return await _ctx.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Files>> GetAllAsync()
        {
            return await _ctx.Files.Include(x => x.Status).AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<Files>> GetPagedFileResults(int page, int pageSize, int? state, string type, DateTime? createdFrom, DateTime? createdTo)
        {
            try
            {
                IQueryable<Files> query = _ctx.Files.Include(x => x.Status);

                if (state.HasValue)
                {
                    query = query.Where(x => x.StatusId == (int) state.Value);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(x => x.Type == type);
                }

                if (createdFrom.HasValue && createdTo.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= createdFrom.Value && x.CreatedDate <= createdTo.Value);
                }
               
                return await query.AsNoTracking().GetPaged(page, pageSize);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        public bool CheckIfFileExistsWithProcessingState(int partnerId)
        {
            return _ctx.Files.Any(x => x.PartnerId == partnerId &&
                                      x.Type == FileType.PartnerTrans.ToString() &&
                                      x.Status.Type == Constants.StatusType.FileStatus
                                      && (x.Status.Name == Constants.Status.Created));
        }

        public async Task<List<Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, TransactionSortOrder sortOrder)
        {
            try
            {
                IQueryable<Files> fileQuery =
                    _ctx.Files
                        .Where(x => x.PartnerId == partnerId && x.Status.Name == Constants.Status.Processed &&
                                    x.PaymentStatus.Name == Constants.Status.Paid &&
                                    x.CreatedDate.Date >= startDate.Date && x.CreatedDate <= endDate.Date);

                if (sortOrder.Equals(TransactionSortOrder.DateAsc))
                {
                    fileQuery = fileQuery.OrderBy(x => x.CreatedDate.Date).ThenBy(x => x.CreatedDate.TimeOfDay);
                }
                else if (sortOrder.Equals(TransactionSortOrder.DateDesc))
                {
                    fileQuery = fileQuery.OrderByDescending(x => x.CreatedDate.Date)
                        .ThenBy(x => x.CreatedDate.TimeOfDay);
                }

                return await fileQuery.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public async Task<PagedResult<Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, int page, int pageSize, TransactionSortOrder sortOrder)
        {
            try
            {
                IQueryable<Files> fileQuery =
                    _ctx.Files
                        .Where(x => x.PartnerId == partnerId && x.Status.Name == Constants.Status.Processed &&
                                    x.PaymentStatus.Name == Constants.Status.Paid &&
                                    x.CreatedDate.Date >= startDate.Date && x.CreatedDate <= endDate.Date);

                if (sortOrder.Equals(TransactionSortOrder.DateAsc))
                {
                    fileQuery = fileQuery.OrderBy(x => x.CreatedDate.Date).ThenBy(x => x.CreatedDate.TimeOfDay);
                }
                else if (sortOrder.Equals(TransactionSortOrder.DateDesc))
                {
                    fileQuery = fileQuery.OrderByDescending(x => x.CreatedDate.Date)
                        .ThenBy(x => x.CreatedDate.TimeOfDay);
                }

                return await fileQuery.GetPaged(page, pageSize);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        #endregion
    }
}
