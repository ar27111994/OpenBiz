using BLL.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Procurement
{
    public class RQItem : AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [Required]
        public long ItemID { get; set; }
        public RawMaterial Material { get; set; }
        [Required]
        public long RFQID { get; set; }
        public RequestForQuotation RFQ { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
