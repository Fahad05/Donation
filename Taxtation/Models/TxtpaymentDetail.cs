using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtpaymentDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PayId { get; set; }
        public double? PaySerialNo { get; set; }
        public int? Coaid { get; set; }
        public DateTime? PayBillDate { get; set; }
        public string PayBillNo { get; set; }
        public double? PayOriginalAmt { get; set; }
        public double? PayAmtOwing { get; set; }
        public double? PayDiscAmt { get; set; }
        public double? PayPaymentAmt { get; set; }
        public double? PayExcAmt { get; set; }
        public int? ExciseId { get; set; }
        public string PayExciseSlab { get; set; }
        public double? PayExcisePercent { get; set; }
        public double? PayExciseAmount { get; set; }
        public int? TaxId { get; set; }
        public double? PayTaxPercent { get; set; }
        public double? PayTaxAmount { get; set; }
        public string PayChqNo { get; set; }
        public DateTime? PayChqDate { get; set; }
        public string PayRemarks { get; set; }
    }
}
