using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtpurchaseDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int? PurId { get; set; }
        public int? PurSerialNo { get; set; }
        public int? ItmId { get; set; }
        public double? PurQty { get; set; }
        public double? PurRate { get; set; }
        public double? PurAmount { get; set; }
        public double? PurDiscountPer { get; set; }
        public double? PurDiscountAmt { get; set; }
        public int? TaxId { get; set; }
        public double? PurTaxPer { get; set; }
        public double? PurTaxAmt { get; set; }
        public double? PurGrossAmt { get; set; }
        public double? PurNetAmt { get; set; }
        public double? PurReturned { get; set; }
        public int? ExciseId { get; set; }
        public double? PurExPer { get; set; }
        public double? PurExAmt { get; set; }
        public string PurRemarks { get; set; }
    }
}