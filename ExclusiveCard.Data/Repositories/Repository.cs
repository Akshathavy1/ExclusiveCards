using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly ExclusiveContext _context;
        private readonly ILogger _logger;

        //        public Repository(ExclusiveContext context) => _context = context;
        public Repository(ExclusiveContext context)
        { 
            _context = context;
            _logger = LogManager.GetLogger(GetType().FullName);
        }

        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Delete(int id)
        {
            var entityToDelete = _context.Set<TEntity>().Find(id);
            if (entityToDelete != null)
            {
                _context.Set<TEntity>().Remove(entityToDelete);
            }
        }

        public void Update(TEntity entity)
        {
            var editedEntity = _context.Set<TEntity>().Update(entity);
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _context.Set<TEntity>().FirstOrDefault(predicate);
            return data;
        }

        public TEntity GetNoTrack(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _context.Set<TEntity>().FirstOrDefault(predicate);
            return data;
        }

        public IList<TEntity> Filter()
        {
            return _context.Set<TEntity>().ToList();
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public IList<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _context.Set<TEntity>().Where(predicate);
            return data?.ToList();
        }

        public async Task<IList<TEntity>> FilterNoTrackAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _context.Set<TEntity>().AsNoTracking().Where(predicate);
            return await data?.ToListAsync();
        }

        public async Task<IList<TEntity>> WhereNoTrackAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            //var data = _context.Set<TEntity>().AsNoTracking().Where(predicate);

            //foreach (var includeExpression in includeExpressions)
            //{
            //    data = data.Include(includeExpression);
            //}
            //return data;
            var dbSet = _context.Set<TEntity>().Where(predicate);

            //IQueryable<TEntity> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                dbSet = dbSet.Include(includeExpression);
            }

            return await dbSet?.ToListAsync();
        }

        public IList<TEntity> FilterNoTrack(Expression<Func<TEntity, bool>> predicate)
        {
            var data = _context.Set<TEntity>().AsNoTracking().Where(predicate);
            return data?.ToList();
        }

        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();

            IQueryable<TEntity> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return query ?? dbSet;
        }

        //public IQueryable<TEntity> AndThenInclude(Expression<Func<TEntity, object>> include, Expression<Func<TEntity, object>> thenInclude)
        //{
        //DbSet<TEntity> dbSet = _context.Set<TEntity>();

        //IQueryable<TEntity> x;

        //var cust = _context.Customer.Include(x => x.MembershipCards).ThenInclude(y => y.CashbackTransactions);

        //var query = dbSet.Include(include).ThenInclude<TEntity>("string2");
        //var y = dbSet.
        //.ThenInclude(thenInclude);

        //return query ?? dbSet;
        //}

        public IQueryable<TEntity> IncludeAndThenInclude(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);

            if (includes != null)
            {
                query = includes(query);
            }
            return query;
        }

        public async Task<PagedResult<TEntity>> PagedResult(int size, int page)
        {
            return await _context.Set<TEntity>().AsNoTracking().GetPaged(page, size);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public DbCommand LoadStoredProcedure(string storedProcName, int commandTimeout = 3000)
        {
            var cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandTimeout = commandTimeout;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }

        public IList<TEntity> ExecuteStoredProcedure<TEntity>(DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return MapToList<TEntity>(reader);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        public DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");
            try
            {

                var param = cmd.CreateParameter();
                param.ParameterName = paramName;
                param.Value = paramValue;
                cmd.Parameters.Add(param);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return cmd;

        }

        private IList<TEntity> MapToList<TEntity>(DbDataReader dr)
        {
            try
            {
                var objList = new List<TEntity>();
                var props = typeof(TEntity).GetRuntimeProperties();
                var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TEntity obj = Activator.CreateInstance<TEntity>();
                        if (props.Any())
                        {
                            foreach (var prop in props)
                            {
                                var propName = prop.Name.ToLower();
                                if (colMapping.ContainsKey(propName))
                                {
                                    var val = dr.GetValue(colMapping[propName].ColumnOrdinal.Value);
                                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                                }
                            }
                        }
                        else
                        {
                            obj = (TEntity)dr.GetValue(0);
                        }
                        objList.Add(obj);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }
    }
}