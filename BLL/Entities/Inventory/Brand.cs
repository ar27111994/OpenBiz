using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Inventory
{
    public class Brand:AuditableEntity
    {
        [Required]
        [StringLength(40,MinimumLength =2)]
        public string BrandName { get; set; }
    }
}