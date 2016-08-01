using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Payment
{
    public class Account:AuditableEntity
    {
        [Required]
        [StringLength(70, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Company { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 9)]
        public string AccountNo { get; set; }

        [Required]
        public int BankID { get; set; }
        public Bank Bank { get; set; }
    }
}
