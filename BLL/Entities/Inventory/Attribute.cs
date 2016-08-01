using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Inventory
{
    public class Attribute:Entity
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        public ICollection<AttributeValue> Values { get; set; }

        [Required]
        public long ProductID { get; set; }
        public Product Product { get; set; }
    }
}
