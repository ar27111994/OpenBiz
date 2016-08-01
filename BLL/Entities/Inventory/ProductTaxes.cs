using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Inventory
{
    public class ProductTaxes : AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [Required]
        public long ProductID { get; set; }
        public Product Product { get; set; }
        [Required]
        public long TaxID { get; set; }
        public Tax Tax { get; set; }
    }
}