using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Transportation
{
    public class ShippingSlip:AuditableEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public long OrderID { get; set; }
        public DistOrders.Order Order { get; set; }
    }
}
