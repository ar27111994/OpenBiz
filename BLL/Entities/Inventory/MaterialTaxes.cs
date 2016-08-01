using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Inventory
{
    public class MaterialTaxes:AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [Required]
        public long MaterialID { get; set; }
        public RawMaterial Material { get; set; }
        [Required]
        public long TaxID { get; set; }
        public Tax Tax { get; set; }
    }
}