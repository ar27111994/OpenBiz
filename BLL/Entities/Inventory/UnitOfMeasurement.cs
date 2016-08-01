using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Inventory
{
    public class UnitOfMeasurement:AuditableEntity
    {
        [Required()]
        [StringLength(30,MinimumLength =3)]
        public string Unit { get; set; }

        public bool IsActive { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<RawMaterial> RawMaterials { get; set; }
    }
}
