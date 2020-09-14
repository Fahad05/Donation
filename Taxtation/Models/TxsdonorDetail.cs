using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsdonorDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int DnrId { get; set; }
        public string DnrName { get; set; }
        public string DnrAddress { get; set; }
        public string DnrPhNo { get; set; }
        public string DnrFaxNo { get; set; }
        public string DnrEmail { get; set; }
        public string DnrNtn { get; set; }
        public string DnrStrn { get; set; }
        public string DnrCity { get; set; }
        public string DnrCountry { get; set; }
        public string DnrDesc { get; set; }
        public bool? DnrActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
