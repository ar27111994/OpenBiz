using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Inventory
{
    public class Part:AuditableEntity
    {
        [Required()]
        [StringLength(30,MinimumLength =3)]
        public string PartName { get; set; }

        public ICollection<ProductParts> ProductParts { get; set; }
    }
}