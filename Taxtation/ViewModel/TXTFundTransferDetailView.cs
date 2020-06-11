using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;
namespace Taxtation.ViewModel
{
    public class TXTFundTransferDetailView
    {
        public TxtfundTransferMaster master { get; set; }
        public List<TxtfundTransferMaster> lstMaster { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public double? TotalDebit { get; set; }
        public double? TotalCredit { get; set; }
        

        public TXTFundTransferDetailView()
        {
            master = new TxtfundTransferMaster();
            lstMaster = new List<TxtfundTransferMaster>();
            lstLedger = new List<Txtledger>();
            lstSite = new List<TxssiteDetail>();
            lstAccount = new List<Txscoadetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            TotalDebit = new double();
            TotalCredit = new double();
        }
    }
}
