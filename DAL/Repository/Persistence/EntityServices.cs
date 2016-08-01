using BLL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repository.Persistence
{
    public class EntityService<T> : IEntityService<T> where T:Entity
    {
        SCMSContext _db;
        IQueryable<T> _dbset;

        public EntityService(SCMSContext db)
        {
            _db = db;
            _dbset = _db.Set<T>();
        }
        
        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);

            list = _dbset
                .AsNoTracking()
                .ToList<T>();
            return list;
        }

        public virtual IList<T> GetList(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);

            list = _dbset
                .AsNoTracking()
                .Where(where)
                .ToList<T>();
            return list;
        }

        public virtual T GetById(long? id,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;

            id.GetValueOrDefault();
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);
            item = _dbset.Where(x => x.Id == id).SingleOrDefault(); //Apply where clause
            return item;
        }

        public virtual async Task<T> GetByIdAsync(long? id,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            id = id.GetValueOrDefault();
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);
            item = await _dbset.Where(x => x.Id == id).SingleOrDefaultAsync(); //Apply where clause
            return item;
        }

        public virtual T GetSingle(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);

            item = _dbset
                .AsNoTracking() //Don't track any changes for the selected item
                .FirstOrDefault(where); //Apply where clause
            return item;
        }

        public virtual void Add(params T[] items)
        {
            foreach (T item in items)
            {
                _db.Entry(item).State = EntityState.Added;
            }
        }

        public virtual void Update(params T[] items)
        {
            foreach (T item in items)
            {
                _db.Entry(item).State = EntityState.Modified;
            }
        }

        public virtual void Remove(params T[] items)
        {
            foreach (T item in items)
            {
                _db.Entry(item).State = EntityState.Deleted;
            }
        }


        public virtual async Task<T> GetSingleAsync(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                _dbset = _dbset.Include<T, object>(navigationProperty);

            item = await _dbset
                .Where(where)
                .AsQueryable()
                .SingleOrDefaultAsync();
            return item;
        }

        public virtual int Commit()
        {
            return _db.SaveChanges();
        }

        public virtual async Task<int> CommitAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
