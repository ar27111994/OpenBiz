using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Orders
{
    public class OrderItems:AuditableEntity
    {
        public long ProductID { get; set; }
        public Inventory.Product Product { get; set; }

        public long OrderID { get; set; }
        public RetailOrder Order { get; set; }
    }
}
