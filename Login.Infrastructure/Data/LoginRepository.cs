using Login.Core.Enum;
using Login.Core.Helpers;
using Login.Core.Interfaces;
using Login.Core.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Login.Infrastructure.Data
{
    public class LoginRepository : IRepository
    {
        private readonly LoginDbContext _dbContext;

        public LoginRepository(LoginDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById<T>(int id) where T : BaseEntity
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public T GetById<T>(int id, string[] navProperties) where T : BaseEntity
        {
            IQueryable<T> result = _dbContext.Set<T>();

            return SetIncludes(result, navProperties).SingleOrDefault(e => e.Id == id);
        }

        public IEnumerable<T> List<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        public PagedResult<T> List<T>(int page, int limit) where T : BaseEntity
        {
            IQueryable<T> result = _dbContext.Set<T>();           
            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> List<T>(string[] navProperties, int page, int limit) where T : BaseEntity
        {
            IQueryable<T> result = _dbContext.Set<T>();
            result = SetIncludes(result, navProperties);
            
            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> List<T>(string[] navProperties, Expression<Func<T, Object>> predicateSort, Order typeOrder, int page, int limit) where T : BaseEntity
        {
            IQueryable<T> result = _dbContext.Set<T>();
            result = SetIncludes(result, navProperties);
            result = GetOrderResult(result, predicateSort, typeOrder.ToString());

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> List<T>(Expression<Func<T, Object>> predicateSort, Order typeOrder, int page, int limit) where T : BaseEntity
        {
            IQueryable<T> result = _dbContext.Set<T>();
            result = GetOrderResult(result, predicateSort, typeOrder.ToString());

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }     

        public IEnumerable<T> Filter<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);

            return result;
        }

        public IEnumerable<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);

            return SetIncludes(result, navProperties);
        }

        public PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, int page, int limit) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties, int page, int limit) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);
            result = SetIncludes(result, navProperties);

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties, Expression<Func<T, Object>> predicateSort, Order typeOrder, int page, int limit) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);        
            result = SetIncludes(result, navProperties);
            result = GetOrderResult(result, predicateSort, typeOrder.ToString());

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, Object>> predicateSort, Order typeOrder, int page, int limit) where T : BaseEntity
        {
            if (predicate == null) return null;
            IQueryable<T> result = _dbContext.Set<T>().Where(predicate);
            result = GetOrderResult(result, predicateSort, typeOrder.ToString());

            IQueryable<T> pagedResult = null;
            int totalPages = 0;
            GetPagedResult(result, page, limit, out pagedResult, out totalPages);

            return new PagedResult<T> { Result = pagedResult, TotalPages = totalPages };
        }

        public void Add<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            var result = _dbContext.Set<T>().Where(predicate);
            _dbContext.Set<T>().UpdateRange(result);
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void DeleteRange<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            var result = _dbContext.Set<T>().Where(predicate);
            _dbContext.Set<T>().RemoveRange(result);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        private IQueryable<T> GetOrderResult<T>(IQueryable<T> result, Expression<Func<T, Object>> predicateSort, string typeOrder)
        {
            if (typeOrder != null && typeOrder.ToUpper() == "DESC")
            {
                result = result.OrderByDescending(predicateSort);
            }
            else
            {
                result = result.OrderBy(predicateSort);
            }

            return result;
        }

        private void GetPagedResult<T>(IQueryable<T> result, int page, int limit, out IQueryable<T> pagedResult, out int totalPages)
        {
            limit = (limit == 0) ? Constants.PaginationLimit : limit;
            pagedResult = null;
            totalPages = 0;

            double total = result.Count();
            if (total > 0)
            {
                page = page * limit;
                totalPages = (int)Math.Ceiling(total / limit);
                pagedResult = result.Skip(page).Take(limit);
            }
        }

        private IQueryable<T> SetIncludes<T>(IQueryable<T> query, string[] navProperties) where T : BaseEntity
        {
            return navProperties.Aggregate(query, (current, include) => current.Include(include));
        }
    }
}
