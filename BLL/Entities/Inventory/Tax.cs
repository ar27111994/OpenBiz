using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Inventory
{
    public class Tax:AuditableEntity
    {
        [StringLength(40,MinimumLength =2)]
        public string Name { get; set; }
        [Range(0,100)]
        public double Percentage { get; set; }

        public ICollection<ProductTaxes> Products { get; set; }
        public ICollection<MaterialTaxes> Materials { get; set; }

    }
}
