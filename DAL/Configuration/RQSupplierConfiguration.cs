

using BLL.Entities.Procurement;
using System.Data.Entity.ModelConfiguration;

namespace OpenBiz.Data.Configuration
{
    public class RQSupplierConfiguration : EntityTypeConfiguration<RQSupplier>
    {
        public RQSupplierConfiguration()
        {
            HasKey(t => new { t.SupplierID, t.RFQID });


            HasRequired(pt => pt.Supplier)
                .WithMany(p => p.RFQs)
                .HasForeignKey(pt => pt.SupplierID);

            HasRequired(pt => pt.RFQ)
                .WithMany(t => t.Suppliers)
                .HasForeignKey(pt => pt.RFQID);
        }
    }
}
