using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsprojectDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int ProId { get; set; }
        public string ProName { get; set; }
        public string ProAbbr { get; set; }
        public string ProDesc { get; set; }
        public bool? ProActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? ProStartDate { get; set; }
        public DateTime? ProEndDate { get; set; }
        public int? ProOrg { get; set; }
        public double? ProBudget { get; set; }
    }
}
