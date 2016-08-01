using BLL.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Warehouse
{
    public class WarehouseMaterials:AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [StringLength(50,MinimumLength =2)]
        public string EntryTitle { get; set; }
        [StringLength(90, MinimumLength = 5)]
        public string Purpose { get; set; }
        [Required]
        public long RawMaterialID { get; set; }
        public RawMaterial RawMaterial { get; set; }
        [Required]
        public long WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; }
        [Range(1,99999999999)]
        public int Quantity { get; set; }

        public DateTime PostingTime { get; set; }

        public enum MaterialMovement { Inbound, Outbound }
        [Required]
        public MaterialMovement EntryType { get; set; }
    }
}
