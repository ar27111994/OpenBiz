using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Distribution
{
    public class Distributor:AuditableEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        [ScaffoldColumn(false)]
        public double Latitude { get; set; }
        [ScaffoldColumn(false)]
        public int Longitude { get; set; }
    }
}
