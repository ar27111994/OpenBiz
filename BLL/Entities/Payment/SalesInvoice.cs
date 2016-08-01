using BLL.Entities.DistOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Payment
{
    public class SalesInvoice:AuditableEntity
    {
        [Required]
        public long OrderID { get; set; }

        public Order Order { get; set; }

        [Required]
        [StringLength(30,MinimumLength =10)]
        [Index("AlteranteKey_InvoiceNo", 1, IsUnique = true)]
        public string InviceNo { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }
    }
}
