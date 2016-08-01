using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Orders
{
    public class RetailOrder:AuditableEntity
    {
        public long ShopID { get; set; }
        public Shop Shop { get; set; }
    }
}
