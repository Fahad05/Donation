using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtdonationDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int DnnId { get; set; }
        public int? DnnDnrId { get; set; }
        public double DnnAmount { get; set; }
        public string DnnType { get; set; }
        public DateTime? DnnDate { get; set; }
        public int? DnnDcaId { get; set; }
        public string DnnDesc { get; set; }
        public bool? DnnTaxable { get; set; }
        public bool? DnnActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public string DnnChequeNo { get; set; }
    }
}
