using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Warehouse
{
    public class Warehouse:AuditableEntity
    {
        [StringLength(50,MinimumLength =2)]
        public string WarehouseName { get; set; }
        [StringLength(150,MinimumLength =4)]
        public string WarehouseLocation { get; set; }
        [ScaffoldColumn(false)]
        public double Longitude { get; set; }
        [ScaffoldColumn(false)]
        public double Latitude { get; set; }

        public ICollection<WarehouseProducts> Products { get; set; }
        public ICollection<WarehouseMaterials> Materials { get; set; }
    }
}