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

        public IEnumerable<T> List<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        public IEnumerable<T> Search<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            if (predicate == null) return null;
            return _dbContext.Set<T>().Where(predicate);
        }

        public T Add<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
