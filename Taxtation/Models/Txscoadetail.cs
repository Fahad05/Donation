using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class Txscoadetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int Coaid { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public string AccAbbr { get; set; }
        public string AccDesc { get; set; }
        public string AccHierarchyCode { get; set; }
        public string AccGrpCode { get; set; }
        public string AccComCode { get; set; }
        public string AccBsuCode { get; set; }
        public string AccParentAccount { get; set; }
        public string AccAccountType { get; set; }
        public string AccAccountNature { get; set; }
        public string AccAccountSubNature { get; set; }
        public bool? AccTaxAccount { get; set; }
        public bool? AccAllowBudgeting { get; set; }
        public bool? AccAllowPosting { get; set; }
        public bool? AccReconcile { get; set; }
        public bool? AccThirdParty { get; set; }
        public DateTime? AccEffectPeriodStart { get; set; }
        public DateTime? AccEffectPeriodEnd { get; set; }
        public bool? AccActive { get; set; }
        public string EditOutSide { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public double? OpeningBalance { get; set; }
        public double? ClosingBalance { get; set; }
        public double? OpeningBalanceCr { get; set; }
        public double? ClosingBalanceCr { get; set; }
        public double? TransactionDr { get; set; }
        public double? TransactionCr { get; set; }
        public string AccLevel { get; set; }
        public double? TopeningBalance { get; set; }
        public double? TclosingBalance { get; set; }
        public double? TopeningBalanceCr { get; set; }
        public double? TclosingBalanceCr { get; set; }
        public double? TtransactionDr { get; set; }
        public double? TtransactionCr { get; set; }
        public bool? Pl { get; set; }
        public bool? Bs { get; set; }
        public bool? Cf { get; set; }
        public bool? Ce { get; set; }
        public bool? Cf1 { get; set; }
        public bool? Cf2 { get; set; }
        public bool? Ce1 { get; set; }
        public bool? Ce2 { get; set; }
        public int? AccLevelNo { get; set; }
        public string AccNameLevelWise { get; set; }
        public string AccAccountTypeShort { get; set; }
        public double? AccOpeningBalance { get; set; }
    }
}
