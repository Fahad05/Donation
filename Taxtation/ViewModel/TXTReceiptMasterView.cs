using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTReceiptMasterView
    {
        public TxtreceiptMaster master { get; set; }
        public List<TxtreceiptMaster> lstMaster { get; set; }
        public List<TxtreceiptDetail> lstDetailSale { get; set; }
        public List<TxtreceiptDetail> lstDetailOther { get; set; }
        public List<Txscoadetail> lstDebit { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<Txscoadetail> lstCustomer { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }

        public TXTReceiptMasterView()
        {
            master = new TxtreceiptMaster();
            lstMaster = new List<TxtreceiptMaster>();
            lstDetailSale = new List<TxtreceiptDetail>();
            lstDebit = new List<Txscoadetail>();
            lstLedger = new List<Txtledger>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstCustomer = new List<Txscoadetail>();
            lstExcise = new List<TxstaxDetail>();
            lstTax = new List<TxstaxDetail>();
            lstAccount = new List<Txscoadetail>();

        }

    }
}
