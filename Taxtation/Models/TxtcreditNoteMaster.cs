using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtcreditNoteMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int ScnId { get; set; }
        public string ScnCode { get; set; }
        public DateTime? ScnDate { get; set; }
        public string ScnGpno { get; set; }
        public DateTime? ScnGpdate { get; set; }
        public string ScnInvoiceNo { get; set; }
        public DateTime? ScnInvoiceDate { get; set; }
        public string ScnCondition { get; set; }
        public string ScnLocType { get; set; }
        public int? StrId { get; set; }
        public int? WrhId { get; set; }
        public int? ShtId { get; set; }
        public string ScnSaleType { get; set; }
        public string ScnSaleCode { get; set; }
        public string ScnRefNo { get; set; }
        public string ScnRemarks { get; set; }
        public int? GrpId { get; set; }
        public int? ComId { get; set; }
        public string ScnPostStatus { get; set; }
        public DateTime? ScnPostDate { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public double? ApprovedLevel { get; set; }
        public int? SitId { get; set; }
        public double? ScnTotalQuantity { get; set; }
        public double? ScnTotalAmount { get; set; }
        public double? ScnExcPaidAmount { get; set; }
        public double? ScnSubAmtTotalMain { get; set; }
        public double? ScnTotalDiscMain { get; set; }
        public double? ScnTotalTaxMain { get; set; }
        public double? ScnTotalPaid { get; set; }
        public double? ScnTotalBalance { get; set; }
        public double? ScnTotalAmountMain { get; set; }
        public int? AccId { get; set; }
        public double? ScnTotalExciseTaxMain { get; set; }
        public string ScnSaleTrNo { get; set; }
        public DateTime? ScnSaleDate { get; set; }
    }
}
