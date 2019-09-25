using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtdebitNoteMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PdnId { get; set; }
        public string PdnCode { get; set; }
        public DateTime? PdnDate { get; set; }
        public string PdnGpno { get; set; }
        public DateTime? PdnGpdate { get; set; }
        public string PdnCondition { get; set; }
        public string PdnLocType { get; set; }
        public int? StrId { get; set; }
        public int? WrhId { get; set; }
        public int? ShtId { get; set; }
        public string PdnPurchaseType { get; set; }
        public string PdnPurchaseCode { get; set; }
        public string PdnRefNo { get; set; }
        public string PdnRemarks { get; set; }
        public int? GrpId { get; set; }
        public int? ComId { get; set; }
        public string PdnPostStatus { get; set; }
        public DateTime? PdnPostDate { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public double? ApprovedLevel { get; set; }
        public string SitId { get; set; }
        public double? PdnSubAmtTotalMain { get; set; }
        public double? PdnTotalDiscMain { get; set; }
        public double? PdnTotalTaxMain { get; set; }
        public double? PdnTotalPaid { get; set; }
        public double? PdnTotalBalance { get; set; }
        public double? PdnTotalAmountMain { get; set; }
        public string AccId { get; set; }
        public string PdnBillNo { get; set; }
        public double? PdnTotalExciseTaxMain { get; set; }
    }
}
