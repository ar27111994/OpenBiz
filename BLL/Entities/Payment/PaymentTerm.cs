using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Payment
{
    public class PaymentTerm:AuditableEntity
    {
        [Required]
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        public string Description { get; set; }

        public ICollection<PaymentTerms> Terms { get; set; }
    }
}
