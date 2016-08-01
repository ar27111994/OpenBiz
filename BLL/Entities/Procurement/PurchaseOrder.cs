using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BLL.Entities.Procurement
{
    public class PurchaseOrder:AuditableEntity
    {
        [Required]
        public int QuoteID { get; set; }
        public Quote Quote { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        public Warehouse.Warehouse Warehouse  { get; set; }
    }
}
