using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTJournalDetailView
    {
        public TxtjournalMaster master { get; set; }
        public List<TxtjournalMaster> lstMaster { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public double? TotalDebit { get; set; }
        public double? TotalCredit { get; set; }
        public List<EFJV> eFJV { get; set; }

        public TXTJournalDetailView()
        {
            master = new TxtjournalMaster();
            lstMaster = new List<TxtjournalMaster>();
            lstLedger = new List<Txtledger>();
            lstSite = new List<TxssiteDetail>();
            lstAccount = new List<Txscoadetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            TotalDebit = new double();
            TotalCredit = new double();
            eFJV = new List<EFJV>();
        }
    }

    public class EFJV
    {
        public double? amount { get; set; }
        public string debitCredit { get; set; }
    }
}
