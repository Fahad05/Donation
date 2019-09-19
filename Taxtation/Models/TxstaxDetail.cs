using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxstaxDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int TaxId { get; set; }
        public string TaxType { get; set; }
        public string TaxName { get; set; }
        public string TaxAbbr { get; set; }
        public double? TaxPercent { get; set; }
        public string TaxDesc { get; set; }
        public bool? TaxActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
