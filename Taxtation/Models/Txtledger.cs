using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class Txtledger
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int Sno { get; set; }
        public double? TrserialNo { get; set; }
        public string Trno { get; set; }
        public DateTime? Trdate { get; set; }
        public DateTime? Trgldate { get; set; }
        public int? SitId { get; set; }
        public string Trtype { get; set; }
        public string TrpostStatus { get; set; }
        public DateTime? TrpostDate { get; set; }
        public string TraccCode { get; set; }
        public string TrrefAccCode { get; set; }
        public string TrdocReference { get; set; }
        public double? Trdebit { get; set; }
        public double? Trcredit { get; set; }
        public int? TrcurId { get; set; }
        public double? TrexchangeRate { get; set; }
        public double? TramountConverted { get; set; }
        public int? TrtxsId { get; set; }
        public double? TrtaxPercent { get; set; }
        public double? TrtaxAmount { get; set; }
        public string TrchequeNo { get; set; }
        public DateTime? TrchequeDate { get; set; }
        public string TrrefNo { get; set; }
        public string Trremarks { get; set; }
        public string TrentryType { get; set; }
        public string EntryFrom { get; set; }
        public double? TropeningBalance { get; set; }
        public double? TrclosingBalance { get; set; }
        public double? RunningBalance { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public string TrentryTypeDoc { get; set; }
        public int? TrcustomerRef { get; set; }
        public int? TrsupplierRef { get; set; }
        public int? TrtaxRef { get; set; }
        public DateTime? TrinvDate { get; set; }
        public string TrinvNo { get; set; }
        public double? TramountWithTax { get; set; }
        public double? Tramount { get; set; }
        public int? TrtxsExciseId { get; set; }
        public double? TrtaxExcisePercent { get; set; }
        public double? TrtaxExciseAmount { get; set; }
    }
}
