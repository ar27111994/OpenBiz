

using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Warehouse
{
    public class WarehouseAdmin:AuditableEntity
    {
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
