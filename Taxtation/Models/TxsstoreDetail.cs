using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsstoreDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int StrId { get; set; }
        public string StrName { get; set; }
        public string StrAbbr { get; set; }
        public string StrDesc { get; set; }
        public bool? StrActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
