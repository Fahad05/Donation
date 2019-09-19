using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtsaleDetail
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int? SalId { get; set; }
        public double? SalSerialNo { get; set; }
        public int? ItmId { get; set; }
        public double? SalQty { get; set; }
        public double? SalRate { get; set; }
        public double? SalAmount { get; set; }
        public double? SalDiscPer { get; set; }
        public double? SalDiscAmt { get; set; }
        public int? TaxId { get; set; }
        public double? SalVatper { get; set; }
        public double? SalGrossAmt { get; set; }
        public double? SalNetAmt { get; set; }
        public double? SalReturned { get; set; }
    }
}
