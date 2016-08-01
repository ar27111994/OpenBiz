using BLL.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configuration
{
    public class MaterialTaxesConfiguration : EntityTypeConfiguration<MaterialTaxes>
    {
        public MaterialTaxesConfiguration()
        {

            HasKey(t => new { t.MaterialID, t.TaxID });


            HasRequired(pt => pt.Material)
                .WithMany(p => p.Taxes)
                .HasForeignKey(pt => pt.MaterialID);

            HasRequired(pt => pt.Tax)
                .WithMany(t => t.Materials)
                .HasForeignKey(pt => pt.TaxID);
        }
    }
}
