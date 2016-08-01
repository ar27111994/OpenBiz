using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Inventory
{
    public class AttributeValue:Entity
    {
        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        public long AttributeID { get; set; }
        public Attribute Attribute { get; set; }
    }
}