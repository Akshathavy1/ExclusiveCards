using System;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
   public static class CommonManager
    {
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query,
                                         int page, int pageSize) where T : class
        {
            PagedResult<T> result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            int skip = 0;
            if (page > 0)
            {
                skip = (page - 1) * pageSize;
            }
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }
    }
}
