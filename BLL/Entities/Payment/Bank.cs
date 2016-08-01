using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Payment
{
    public class Bank:AuditableEntity
    {
        [Required]
        [StringLength(70,MinimumLength =2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Address { get; set; }

        [ScaffoldColumn(false)]
        public double Latitude { get; set; }

        [ScaffoldColumn(false)]
        public double Longitude { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
