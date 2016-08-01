using Autofac;
using DAL.Repository.Persistence;

namespace DAL.Configuration
{
    public class DI:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SCMSContext>().AsSelf().InstancePerRequest();

            builder.RegisterGeneric(typeof(EntityService<>)).AsSelf();

            builder.RegisterGeneric(typeof(EntityService<>)).As(typeof(IEntityService<>)).InstancePerRequest();

            base.Load(builder);
        }
    }
}
