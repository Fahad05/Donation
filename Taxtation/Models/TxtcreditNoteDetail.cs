using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtcreditNoteDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int ScnId { get; set; }
        public string ScnCode { get; set; }
        public double? ScnSerialNo { get; set; }
        public int? ItmId { get; set; }
        public double? ScnSaleQuantity { get; set; }
        public double? ScnReceivedQuantity { get; set; }
        public double? ScnRecQuantity { get; set; }
        public double? ScnRecWeight { get; set; }
        public double? ScnBalQuantity { get; set; }
        public string ScnSubRemarks { get; set; }
        public DateTime? ScnExpiryDate { get; set; }
        public string ScnBatchNo { get; set; }
        public double? ScnRecAmount { get; set; }
        public double? ScnAmount { get; set; }
        public double? ScnTaxAmt { get; set; }
        public double? ScnDiscountAmount { get; set; }
        public double? ScnNetAmount { get; set; }
        public double? ScnExciseTaxAmt { get; set; }
    }
}
