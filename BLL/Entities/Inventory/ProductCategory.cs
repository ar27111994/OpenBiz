using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Inventory
{
    public class ProductCategory:AuditableEntity
    {
        [Required]
        [StringLength(29,MinimumLength =5)]
        public string CategoryName { get; set; }
    }
}