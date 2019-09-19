using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxssiteDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int SitId { get; set; }
        public string SitName { get; set; }
        public string SitAbbr { get; set; }
        public string SitDesc { get; set; }
        public bool? SitActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
