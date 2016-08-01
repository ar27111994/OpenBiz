using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Payment
{
    public class Term:AuditableEntity
    {
        public enum Type { Percentage, FixedAmount }

        [Required]

        public Type TermType { get; set; }

        public double Amount { get; set; }
    }
}