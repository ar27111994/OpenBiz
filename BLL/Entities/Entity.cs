
namespace BLL.Entities
{

    public interface IEntity
    {
        long Id { get; set; }
    }


    public abstract class Entity : IEntity
    {
        public virtual long Id { get; set; }
    }
}
