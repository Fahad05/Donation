using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxscurrencyDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int CurId { get; set; }
        public string CurName { get; set; }
        public string CurAbbr { get; set; }
        public bool? CurIsLocal { get; set; }
        public bool? CurActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public double? CurExchangeRate { get; set; }
    }
}
