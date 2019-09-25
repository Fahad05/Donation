using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtdebitNoteDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PdnId { get; set; }
        public string PdnCode { get; set; }
        public double? PdnSerialNo { get; set; }
        public int? ItmId { get; set; }
        public double? PdnPoquantity { get; set; }
        public double? PdnDeliveredQuantity { get; set; }
        public double? PdnDelQuantity { get; set; }
        public double? PdnDelWeight { get; set; }
        public double? PdnBalQuantity { get; set; }
        public string PdnSubRemarks { get; set; }
        public DateTime? PdnExpiryDate { get; set; }
        public string PdnBatchNo { get; set; }
        public double? PdnAmount { get; set; }
        public double? PdnTaxAmt { get; set; }
        public double? PdnDiscountAmount { get; set; }
        public double? PdnNetAmount { get; set; }
        public double? PdnExciseTaxAmt { get; set; }
    }
}
