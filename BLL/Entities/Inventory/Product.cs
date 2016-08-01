using BLL.Entities.Warehouse;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Inventory
{
    public class Product:AuditableEntity
    {
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(50,MinimumLength =3)]
        [Index("AlteranteKey_SKU", 1, IsUnique = true)]
        public string SKU { get; set; }
        [Range(0.0,99999999999999.9999999)]
        public decimal ProductBasePrice { get; set; }
        [Required]
        [AllowHtml]
        public string ProductDescription { get; set; }

        [Required]
        public long CategoryID { get; set; }
        public ProductCategory Category { get; set; }

        public ICollection<ProductTaxes> Taxes { get; set; }
        public string Barcode { get; set; }

        [Required]
        public long UnitOfMeasurementID { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }

        [Required]
        public long BrandID { get; set; }
        public Brand Brand { get; set; }

        public ICollection<WarehouseProducts> Warehouses { get; set; }
        public ICollection<ProductParts> Parts { get; set; }
        public ICollection<ProductMaterials> RawMaterials { get; set; }
    }
}