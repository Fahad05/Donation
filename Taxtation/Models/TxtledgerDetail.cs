using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtledgerDetail
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
        public string TrpostStatusReverse { get; set; }
        public DateTime? TrpostDateReverse { get; set; }
        public bool? TrreverseStatus { get; set; }
        public DateTime? TrreverseDate { get; set; }
        public double? TrserialNo { get; set; }
        public int? TraccId { get; set; }
        public int? TrrefAccId { get; set; }
        public string TrdocReference { get; set; }
        public double? TrdocRefTotalAmount { get; set; }
        public double? TrdocRefBalanceAmount { get; set; }
        public double? Trdebit { get; set; }
        public double? Trcredit { get; set; }
        public double? TrexchangeRate { get; set; }
        public double? TramountConverted { get; set; }
        public int? TrcurId { get; set; }
        public int? TrtxsId { get; set; }
        public double? TrtaxPercent { get; set; }
        public string TrchequeNo { get; set; }
        public DateTime? TrchequeDate { get; set; }
        public DateTime? TrchequeMaturityDate { get; set; }
        public DateTime? TrchequeDepositDate { get; set; }
        public string TrdepositNo { get; set; }
        public string TrchequeBankName { get; set; }
        public string TrchequeBranchName { get; set; }
        public string TrchequeAccountNo { get; set; }
        public string TrrefNo { get; set; }
        public int? TrgrpId { get; set; }
        public int? TrcomId { get; set; }
        public int? TrbsuId { get; set; }
        public int? TrcosId { get; set; }
        public int? TrdepId { get; set; }
        public int? TrsecId { get; set; }
        public int? TrproId { get; set; }
        public int? TrfucId { get; set; }
        public int? TrcusAccId { get; set; }
        public int? TrsupAccId { get; set; }
        public int? TrconAccId { get; set; }
        public int? TragtAccId { get; set; }
        public int? TrivtAccId { get; set; }
        public int? TritmId { get; set; }
        public string Trremarks { get; set; }
        public string TrentryType { get; set; }
        public double? TropeningBalance { get; set; }
        public double? TrclosingBalance { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public string EntryFrom { get; set; }
        public string EntryTypeDoc { get; set; }
        public double? RunningBalance { get; set; }
        public int? TrdisAccId { get; set; }
        public int? SitId { get; set; }
        public int? StrId { get; set; }
        public DateTime? Pstdate { get; set; }
        public double? TrtaxAmount { get; set; }
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
        public int? BrcId { get; set; }
    }
}
