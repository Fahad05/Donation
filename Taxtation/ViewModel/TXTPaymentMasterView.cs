using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTPaymentMasterView
    {
        public TxtpaymentMaster master { get; set; }
        public List<TxtpaymentMaster> lstMaster { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<Txscoadetail> lstSupplier { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
    }
}
