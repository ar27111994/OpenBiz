using BLL.Entities;
using BLL.Entities.UserAccounts;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Persistence
{
    public static class GetDb
    {
        private static IdentityDbContext<ApplicationUser> dbContext;

        public static SCMSContext Context()
        {
            dbContext = new SCMSContext();
            return dbContext as SCMSContext;
        }

        public static TEntity Repository<TEntity>() where TEntity:Entity
        {
            IEntityService<TEntity> repository = new EntityService<TEntity>(Context());
            return repository as TEntity;
        }

        public static List<PropertyInfo> GetNavigationProperties<T>(T entity) where T : Entity
        {
            var t = entity.GetType();
            var elementType = ((IObjectContextAdapter)dbContext).ObjectContext.CreateObjectSet<T>().EntitySet.ElementType;
            return elementType.NavigationProperties.Select(np => t.GetProperty(np.Name)).ToList();
        }
    }
}
