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
        public string SalDisType { get; set; }
        public int? ExciseId { get; set; }
        public double? SalExcisePer { get; set; }
        public double? SalTaxAmount { get; set; }
        public double? SalReturnQuantity { get; set; }
        public double? SalReturnTax { get; set; }
        public string SalSubRemarks { get; set; }
        public double? SalExAmt { get; set; }
        public double? SalPaidAmt { get; set; }
        public double? SalBalAmt { get; set; }
    }
}
