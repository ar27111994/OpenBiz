using BLL.Entities.Procurement;
using BLL.Entities.Warehouse;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Inventory
{
    public class RawMaterial:AuditableEntity
    {

        [Required()]
        [StringLength(40,MinimumLength =3)]
        public string MaterialName { get; set; }

        [Required()]
        [StringLength(40, MinimumLength = 2)]
        public string Manufacturer { get; set; }

        [Required()]
        public long SupplierID { get; set; }
        public Supplier Supplier { get; set; }

        [Required()]
        [Range(0, 9999999999)]
        public long MaterialPrice { get; set; }

        public string Barcode { get; set; }

        [Required()]
        public long UnitOfMeasurementID { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }

        public ICollection<WarehouseMaterials> Warehouses { get; set; }
        public ICollection<MaterialTaxes> Taxes { get; set; }

        public ICollection<ProductMaterials> Products { get; set; }

        public ICollection<RQItem> RFQs { get; set; }
    }
}