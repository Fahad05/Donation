using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtfundTransferMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int TrId { get; set; }
        public string Trno { get; set; }
        public DateTime? Trdate { get; set; }
        public DateTime? Trgldate { get; set; }
        public int? SitId { get; set; }
        public double? TrtotalAmount { get; set; }
        public string TrmainRemarks { get; set; }
        public double? TrexchangeRate { get; set; }
        public int? TrcurId { get; set; }
        public int TrgrpId { get; set; }
        public int? TrcomId { get; set; }
        public bool? TrreverseStatus { get; set; }
        public DateTime? TrreverseDate { get; set; }
        public string TrpostStatus { get; set; }
        public DateTime? TrpostDate { get; set; }
        public string TrpostStatusReverse { get; set; }
        public DateTime? TrpostDateReverse { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public double? ApprovedLevel { get; set; }
        public string TrrefMain { get; set; }
    }
}
