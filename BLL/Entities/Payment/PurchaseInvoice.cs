﻿using BLL.Entities.Procurement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities.Payment
{
    public class PurchaseInvoice : AuditableEntity
    {
        [Required]
        public long OrderID { get; set; }

        public PurchaseOrder Order { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 10)]
        [Index("AlteranteKey_InvoiceNo", 1, IsUnique = true)]
        public string InviceNo { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }
    }
}
