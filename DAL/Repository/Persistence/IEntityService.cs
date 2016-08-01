using BLL.Entities;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repository.Persistence
{
    public interface IEntityService<T> where T:Entity
    {
        IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        void Add(params T[] items);
        void Update(params T[] items);
        void Remove(params T[] items);
        T GetById(long? id, params Expression<Func<T, object>>[] navigationProperties);
        Task<T> GetByIdAsync(long? id, params Expression<Func<T, object>>[] navigationProperties);
        Task<T> GetSingleAsync(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties);
        int Commit();
        Task<int> CommitAsync();
        void Dispose();
    }
}
