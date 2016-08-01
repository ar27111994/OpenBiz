using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.DistOrders
{
    public class Order:AuditableEntity
    {
        public long DistributorID { get; set; }
        public Distribution.Distributor Distributor { get; set; }
    }
}
