using BLL.Entities.Inventory;
using System.Data.Entity.ModelConfiguration;

namespace DAL.Configuration
{
    class ProductMaterialsConfiguration : EntityTypeConfiguration<ProductMaterials>
    {
        public ProductMaterialsConfiguration()
        {

            HasKey(t => new { t.ProductionPlanID, t.ProductID, t.MaterialID });

            HasRequired(pt => pt.ProductionPlan)
                .WithMany(p => p.RawMaterials)
                .HasForeignKey(pt => pt.ProductionPlanID)
                .WillCascadeOnDelete(false);


            HasRequired(pt => pt.Product)
                .WithMany(p => p.RawMaterials)
                .HasForeignKey(pt => pt.ProductID);

            HasRequired(pt => pt.Material)
                .WithMany(t => t.Products)
                .HasForeignKey(pt => pt.MaterialID);
        }
    }
}
