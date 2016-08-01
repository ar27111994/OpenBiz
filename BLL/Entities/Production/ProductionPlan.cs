using BLL.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Production
{
    public class ProductionPlan:AuditableEntity
    {
        [Required]
        [StringLength(70,MinimumLength =2)]
        public string Title { get; set; }

        [Required]
        public long FinishedProductID { get; set; }
        public Product FinishedProduct { get; set; }

        [Required]
        [Range(1,9999999)]
        public int Quantity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ICollection<ProductParts> Parts { get; set; }

        public ICollection<ProductMaterials> RawMaterials { get; set; }
    }
}
