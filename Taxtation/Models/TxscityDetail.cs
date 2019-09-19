using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxscityDetail
    {
        public string CtyCode { get; set; }
        public string CtyName { get; set; }
        public string CtyAbbr { get; set; }
        public string CtyDesc { get; set; }
        public string CouCode { get; set; }
        public string SttCode { get; set; }
        public bool? CtyActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
