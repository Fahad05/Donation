using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxscountryDetail
    {
        public string CouCode { get; set; }
        public string CouName { get; set; }
        public string CouAbbr { get; set; }
        public string CouDesc { get; set; }
        public bool? CouActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public bool? CouGcc { get; set; }
    }
}
