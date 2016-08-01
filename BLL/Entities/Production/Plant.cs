using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Production
{
    public class Plant:AuditableEntity
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(170, MinimumLength = 4)]
        public string Address { get; set; }
        [ScaffoldColumn(false)]
        public double Latitude { get; set; }
        [ScaffoldColumn(false)]
        public double Longitude { get; set; }
        
        public enum PlantType { Assembly, Manufacturing }
        public PlantType Type { get; set; }
    }
}
