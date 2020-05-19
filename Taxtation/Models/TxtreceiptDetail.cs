using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtreceiptDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int RecId { get; set; }
        public double? RecSerialNo { get; set; }
        public int? Coaid { get; set; }
        public DateTime? RecInvDate { get; set; }
        public string RecInvNo { get; set; }
        public double? RecOriginalAmt { get; set; }
        public double? RecAmtOwing { get; set; }
        public double? RecDiscAmt { get; set; }
        public double? RecReceiptAmt { get; set; }
        public double? RecExcAmt { get; set; }
        public int? ExciseId { get; set; }
        public string RecExciseSlab { get; set; }
        public double? RecExcisePercent { get; set; }
        public double? RecExciseAmount { get; set; }
        public int? TaxId { get; set; }
        public double? RecTaxPercent { get; set; }
        public double? RecTaxAmount { get; set; }
        public string RecChqNo { get; set; }
        public DateTime? RecChqDate { get; set; }
        public string RecRemarks { get; set; }
    }
}
