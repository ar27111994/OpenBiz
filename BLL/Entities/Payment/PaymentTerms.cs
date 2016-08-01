using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Entities.Payment
{
    public class PaymentTerms:AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public override long Id { get; set; }

        [Required]
        public long PaymentTermID { get; set; }
        public PaymentTerm PaymentTerm { get; set; }

        [Required]
        public long TermID { get; set; }
        public Term Term { get; set; }
    }
}