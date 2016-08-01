using BLL.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configuration
{
    public class ProductTaxesConfiguration : EntityTypeConfiguration<ProductTaxes>
    {
        public ProductTaxesConfiguration()
        {

            HasKey(t => new { t.ProductID, t.TaxID });


            HasRequired(pt => pt.Product)
                .WithMany(p => p.Taxes)
                .HasForeignKey(pt => pt.ProductID);

            HasRequired(pt => pt.Tax)
                .WithMany(t => t.Products)
                .HasForeignKey(pt => pt.TaxID);
        }
    }
}
