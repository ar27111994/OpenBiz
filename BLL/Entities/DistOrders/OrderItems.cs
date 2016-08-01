using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.DistOrders
{
    class OrderItem:AuditableEntity
    {
        public long ProductID { get; set; }
        public Inventory.Product Product { get; set; }

        public long OrderID { get; set; }
        public Order Order { get; set; }
    }
}
