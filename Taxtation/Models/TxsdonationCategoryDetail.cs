using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsdonationCategoryDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int DcaId { get; set; }
        public string DcaName { get; set; }
        public string DcaAbbr { get; set; }
        public string DcaDesc { get; set; }
        public bool? DcaActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
