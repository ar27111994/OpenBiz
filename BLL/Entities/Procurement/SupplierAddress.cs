using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Procurement
{
    public class SupplierAddress:AuditableEntity
    {
        [Required]
        [StringLength(170,MinimumLength =4)]
        public string Address { get; set; }
        [ScaffoldColumn(false)]
        public double Latitude { get; set; }
        [ScaffoldColumn(false)]
        public double Longitude { get; set; }
        [Required]
        public long AddressTypeID { get; set; }
        public AddressType AddressType { get; set; }
        [Phone]
        public string PhoneNo { get; set; }
        [Required]
        public long SupplierID { get; set; }
        public Supplier Supplier { get; set; }
    }
}