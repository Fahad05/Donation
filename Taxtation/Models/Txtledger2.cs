using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class Txtledger2
    {
        public int Sno { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public int TrId { get; set; }
        public string Trno { get; set; }
        public DateTime? Trdate { get; set; }
        public DateTime? Trgldate { get; set; }
        public string Trtype { get; set; }
        public string TrpostStatus { get; set; }
        public DateTime? TrpostDate { get; set; }
        public double? TrserialNo { get; set; }
        public int? TraccId { get; set; }
        public int? TrrefAccId { get; set; }
        public string TrdocReference { get; set; }
        public double? Trdebit { get; set; }
        public double? Trcredit { get; set; }
        public double? TrexchangeRate { get; set; }
        public double? TramountConverted { get; set; }
        public int? TrcurId { get; set; }
        public int? TrtxsId { get; set; }
        public double? TrtaxPercent { get; set; }
        public double? TrtaxAmount { get; set; }
        public string TrdepositNo { get; set; }
        public string TrchequeNo { get; set; }
        public DateTime? TrchequeDate { get; set; }
        public DateTime? TrchequeDepositDate { get; set; }
        public string TrchequeBankName { get; set; }
        public string TrchequeBranchName { get; set; }
        public string TrchequeAccountNo { get; set; }
        public string TrrefNo { get; set; }
        public string Trremarks { get; set; }
        public string TrentryType { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? TrinvDate { get; set; }
        public int? TrinvId { get; set; }
        public double? TrinvAmount { get; set; }
        public double? TrinvBalAmount { get; set; }
        public double? TrinvDisAmount { get; set; }
        public string TmpSupplier { get; set; }
        public string TmpCustomer { get; set; }
        public bool? Trrecon { get; set; }
        public string Trstatus { get; set; }
        public DateTime? Trpddate { get; set; }
        public string TrbarCode { get; set; }
    }
}
