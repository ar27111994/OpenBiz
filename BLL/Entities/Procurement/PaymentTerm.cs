using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Procurement
{
    public class PaymentTerm : AuditableEntity
    {
        [Required]
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        public string Description { get; set; }
        [Range(1,1000)]
        public int Days { get; set; }
    }
}