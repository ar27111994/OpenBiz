

using BLL.Entities.Inventory;
using BLL.Entities.Warehouse;
using System.Data.Entity.ModelConfiguration;

namespace OpenBiz.Data.Configuration
{
    public class WarehouseMaterialsConfiguration:EntityTypeConfiguration<WarehouseMaterials>
    {
        public WarehouseMaterialsConfiguration()
        {

            HasKey(t => new { t.Id });


            HasRequired(pt => pt.RawMaterial)
                 .WithMany(p => p.Warehouses)
                 .HasForeignKey(pt => pt.RawMaterialID);


            HasRequired(pt => pt.Warehouse)
                .WithMany(t => t.Materials)
                .HasForeignKey(pt => pt.WarehouseID);

        }
    }
}
