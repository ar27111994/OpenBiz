using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Procurement
{
    public class Supplier:AuditableEntity
    {
        [Required]
        public string SupplierName { get; set; }

        [AllowHtml]
        public string SupplierDetails { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public ICollection<SupplierAddress> SupplierAddress { get; set; }
        public ICollection<Quote> Quotes { get; set; }
        public ICollection<SupplierAddress> Address { get; set; }
        public ICollection<RQSupplier> RFQs { get; set; }

    }
}