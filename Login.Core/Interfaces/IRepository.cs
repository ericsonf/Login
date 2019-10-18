using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Login.Core.Helpers;
using Login.Core.Shared;

namespace Login.Core.Interfaces
{
    public interface IRepository
    {
        T GetById<T>(int id) where T : BaseEntity;

        T GetById<T>(int id, string[] navProperties = null) where T : BaseEntity;

        IEnumerable<T> List<T>() where T : BaseEntity;

        PagedResult<T> List<T>(int page, int limit) where T : BaseEntity;

        PagedResult<T> List<T>(string[] navProperties, int page, int limit) where T : BaseEntity;

        PagedResult<T> List<T>(string[] navProperties, Expression<Func<T, Object>> predicateSort, Enum typeOrder, int page, int limit) where T : BaseEntity;

        PagedResult<T> List<T>(Expression<Func<T, Object>> predicateSort, Enum typeOrder, int page, int limit) where T : BaseEntity;

        IEnumerable<T> Filter<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        IEnumerable<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties) where T : BaseEntity;

        PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, int page, int limit) where T : BaseEntity;

        PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties, int page, int limit) where T : BaseEntity;

        PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] navProperties, Expression<Func<T, Object>> predicateSort, Enum typeOrder, int page, int limit) where T : BaseEntity;

        PagedResult<T> Filter<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, Object>> predicateSort, Enum typeOrder, int page, int limit) where T : BaseEntity;

        void Add<T>(T entity) where T : BaseEntity;

        void Update<T>(T entity) where T : BaseEntity;

        void UpdateRange<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        void Delete<T>(T entity) where T : BaseEntity;

        void DeleteRange<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        void Save();
    }
}