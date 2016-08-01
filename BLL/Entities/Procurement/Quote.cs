using BLL.Entities.Inventory;
using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Entities.Procurement
{
    public class Quote:AuditableEntity
    {
        [Required]
        [AllowHtml]
        public string QuoteDescription { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime QuotationDate { get; set; }
        [Required]
        public long MaterialID { get; set; }
        public RawMaterial RawMaterial { get; set; }

        //public bool Status { get; set; }
        [Required]
        public long RFQID { get; set; }
        public RequestForQuotation RFQ { get; set; }
        [Required]
        public long PaymentTermID { get; set; }
        public PaymentTerm BuyingTerms { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public int TotalCharges { get; set; }
    }
}