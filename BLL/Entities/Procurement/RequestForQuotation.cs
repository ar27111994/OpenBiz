using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Procurement
{
    public class RequestForQuotation:AuditableEntity
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; }
        [Required]
        public long PaymentTermID{ get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public ICollection<RQItem> Items { get; set; }
        public ICollection<RQSupplier> Suppliers { get; set; }
    }
}
