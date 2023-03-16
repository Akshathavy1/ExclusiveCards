using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace ExclusiveCard.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Create(TEntity entity);

        void Delete(TEntity entity);

        void Delete(int id);

        void Update(TEntity entity);

        TEntity GetById(int id);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        TEntity GetNoTrack(Expression<Func<TEntity, bool>> predicate);

        IList<TEntity> Filter();
        IList<TEntity> GetAll();

        Task<IList<TEntity>> GetAllAsync();

        IList<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

        IList<TEntity> FilterNoTrack(Expression<Func<TEntity, bool>> predicate);

        Task<IList<TEntity>> FilterNoTrackAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IList<TEntity>> WhereNoTrackAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeExpressions);

        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeExpressions);

        IQueryable<TEntity> IncludeAndThenInclude(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        Task<PagedResult<TEntity>> PagedResult(int size, int page);

        void SaveChanges();

        Task<int> SaveChangesAsync();

        DbCommand LoadStoredProcedure(string storedProcName, int commandTimeout = 3000);

        IList<TEntity> ExecuteStoredProcedure<TEntity>(DbCommand command);

        //DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue, ParameterDirection direction = ParameterDirection.Input);
        DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue);
    }
}