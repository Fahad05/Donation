using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtreceiptMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int RecId { get; set; }
        public string Trno { get; set; }
        public DateTime? Trdate { get; set; }
        public DateTime? Trgldate { get; set; }
        public int? SitId { get; set; }
        public double? TrtotalAmount { get; set; }
        public string Trtype { get; set; }
        public string TrtypeAccount { get; set; }
        public string TrtypeAccountName { get; set; }
        public string TrmainRemarks { get; set; }
        public double? TrexchangeRate { get; set; }
        public int? CurId { get; set; }
        public string Trmode { get; set; }
        public string TrrefMain { get; set; }
        public string TrcusAccCode { get; set; }
        public double? TrtotalTax { get; set; }
        public double? TrsubTotal { get; set; }
        public double? TrtotalTaxExcise { get; set; }
        public bool? TrreverseStatus { get; set; }
        public DateTime? TrreverseDate { get; set; }
        public string TrpostStatus { get; set; }
        public DateTime? TrpostDate { get; set; }
        public string TrpostStatusReverse { get; set; }
        public DateTime? TrpostDateReverse { get; set; }
        public double? ApprovedLevel { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
