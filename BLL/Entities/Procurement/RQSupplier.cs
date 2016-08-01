using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Procurement
{
    public class RQSupplier : AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }
        [Required]
        public long SupplierID { get; set; }
        public Supplier Supplier { get; set; }
        [Required]
        public long RFQID { get; set; }
        public RequestForQuotation RFQ { get; set; }
    }
}
