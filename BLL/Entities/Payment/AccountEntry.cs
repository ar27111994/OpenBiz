using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Payment
{
    public class AccountEntry:AuditableEntity
    {
        [Required]
        [Range(0,99999999999)]
        public int Amount { get; set; }
        public enum Type { Debit, Credit }
        [Required]
        public Type EntryType { get; set; }

        [Required]
        public long AccountID { get; set; }
        public Account Account { get; set; }
    }
}
