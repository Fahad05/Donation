using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsbankDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int BnkId { get; set; }
        public string BnkType { get; set; }
        public string BnkName { get; set; }
        public string BnkAbbr { get; set; }
        public string BnkBranch { get; set; }
        public string BnkAccNo { get; set; }
        public string BnkPhNo { get; set; }
        public string BnkFax { get; set; }
        public string BnkUrl { get; set; }
        public string BnkEmail { get; set; }
        public string BnkDesc { get; set; }
        public bool? BnkActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
