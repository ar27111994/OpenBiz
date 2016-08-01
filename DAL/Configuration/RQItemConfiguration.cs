

using BLL.Entities.Procurement;
using System.Data.Entity.ModelConfiguration;

namespace OpenBiz.Data.Configuration
{
    public class RQItemConfiguration:EntityTypeConfiguration<RQItem>
    {
        public RQItemConfiguration()
        {
            HasKey(t => new { t.ItemID, t.RFQID });


            HasRequired(pt => pt.Material)
                .WithMany(p => p.RFQs)
                .HasForeignKey(pt => pt.ItemID);

            HasRequired(pt => pt.RFQ)
                .WithMany(t => t.Items)
                .HasForeignKey(pt => pt.RFQID);
        }
    }
}

