using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtsaleMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int SalId { get; set; }
        public string SalSoRef { get; set; }
        public string SalLporef { get; set; }
        public DateTime? SalDate { get; set; }
        public DateTime? SalSupplyDate { get; set; }
        public string SalType { get; set; }
        public int? CusId { get; set; }
        public string SalPayTerms { get; set; }
        public int? BnkId { get; set; }
        public string SalChqNo { get; set; }
        public DateTime? SalChqDate { get; set; }
        public string SalScope { get; set; }
        public string SalPrices { get; set; }
        public string SalRemarks { get; set; }
        public bool? SalActive { get; set; }
        public string SalSalesMapCo { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public int? CurId { get; set; }
        public int? StrId { get; set; }
    }
}
