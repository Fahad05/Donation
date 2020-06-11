using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;
namespace Taxtation.ViewModel
{
    public class TXTOpeningDetailView
    {
        public TxtopeningMaster master { get; set; }
        public List<TxtopeningMaster> lstMaster { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public double? TotalDebit { get; set; }
        public double? TotalCredit { get; set; }
        public List<EFOD> eFOD { get; set; }

        public TXTOpeningDetailView()
        {
            master = new TxtopeningMaster();
            lstMaster = new List<TxtopeningMaster>();
            lstLedger = new List<Txtledger>();
            lstSite = new List<TxssiteDetail>();
            lstAccount = new List<Txscoadetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            TotalDebit = new double();
            TotalCredit = new double();
            eFOD = new List<EFOD>();
        }
    }

    public class EFOD
    {
        public double? amount { get; set; }
        public string debitCredit { get; set; }
    }
}
