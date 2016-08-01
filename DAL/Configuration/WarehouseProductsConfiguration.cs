

using BLL.Entities.Inventory;
using BLL.Entities.Warehouse;
using System.Data.Entity.ModelConfiguration;

namespace OpenBiz.Data.Configuration
{
    public class WarehouseProductsConfiguration : EntityTypeConfiguration<WarehouseProducts>
    {
        public WarehouseProductsConfiguration()
        {

            HasKey(t => new { t.Id });


            HasRequired(pt => pt.Product)
                         .WithMany(p => p.Warehouses)
                         .HasForeignKey(pt => pt.ProductID);

            HasRequired(pt => pt.Warehouse)
                 .WithMany(t => t.Products)
                 .HasForeignKey(pt => pt.WarehouseID);
        }
    }
}
