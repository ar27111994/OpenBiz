using BLL.Entities.Production;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Inventory
{
    public class ProductMaterials : AuditableEntity
    {
        [Required]
        public long ProductionPlanID { get; set; }
        public ProductionPlan ProductionPlan { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [Required]
        public long ProductID { get; set; }
        public Product Product { get; set; }
        [Required]
        public long MaterialID { get; set; }
        public RawMaterial Material { get; set; }
    }
}