using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Login.Core.Shared;

namespace Login.Core.Interfaces
{
    public interface IRepository
    {
        T GetById<T>(int id) where T : BaseEntity;

        IEnumerable<T> List<T>() where T : BaseEntity;

        IEnumerable<T> Search<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity;

        T Add<T>(T entity) where T : BaseEntity;

        void Update<T>(T entity) where T : BaseEntity;

        void Delete<T>(T entity) where T : BaseEntity;
    }
}