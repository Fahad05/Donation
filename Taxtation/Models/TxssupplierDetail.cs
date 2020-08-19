using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxssupplierDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int SupId { get; set; }
        public string SupType { get; set; }
        public string SupName { get; set; }
        public string SupAbbr { get; set; }
        public string SupPerson { get; set; }
        public string SupAddress { get; set; }
        public string SupPhNo { get; set; }
        public string SupFaxNo { get; set; }
        public string SupEmail { get; set; }
        public string SupNtn { get; set; }
        public string SupStrn { get; set; }
        public string SupCity { get; set; }
        public string SupCountry { get; set; }
        public string SupDesc { get; set; }
        public bool? SupActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public int? SupCrDays { get; set; }
    }
}
