using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Entities.Procurement
{
    public class Contract:AuditableEntity
    {
        [Required]
        [StringLength(33,MinimumLength = 7)]
        public string ContractTitle { get; set; }
        [Required]
        [AllowHtml]
        public string ContractDescription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ContractDate { get; set; }
        public bool Renewable { get; set; }
    }
}
