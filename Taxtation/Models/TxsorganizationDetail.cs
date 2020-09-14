using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsorganizationDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgAddress { get; set; }
        public string OrgPhNo { get; set; }
        public string OrgFaxNo { get; set; }
        public string OrgEmail { get; set; }
        public string OrgNtn { get; set; }
        public string OrgStrn { get; set; }
        public string OrgCity { get; set; }
        public string OrgCountry { get; set; }
        public string OrgDesc { get; set; }
        public bool? OrgActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
