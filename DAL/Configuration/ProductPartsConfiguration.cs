using BLL.Entities.Inventory;
using System.Data.Entity.ModelConfiguration;

namespace OpenBiz.Data.Configuration
{
    public class ProductPartsConfiguration: EntityTypeConfiguration<ProductParts>
    {
        public ProductPartsConfiguration()
        {

            HasKey(t => new { t.ProductionPlanID, t.ProductID, t.PartID });

            HasRequired(pt => pt.ProductionPlan)
                .WithMany(p => p.Parts)
                .HasForeignKey(pt => pt.ProductionPlanID)
                .WillCascadeOnDelete(false);

            HasRequired(pt => pt.Product)
                .WithMany(p => p.Parts)
                .HasForeignKey(pt => pt.ProductID);

            HasRequired(pt => pt.Part)
                .WithMany(t => t.ProductParts)
                .HasForeignKey(pt => pt.PartID);
        }
    }
}
