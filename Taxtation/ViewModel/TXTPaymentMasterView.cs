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
        public List<TxtpaymentDetail> lstDetailPurchase { get; set; }
        public List<TxtpaymentDetail> lstDetailOther { get; set; }
        public List<Txscoadetail> lstCredit { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<Txscoadetail> lstSupplier { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }


        public TXTPaymentMasterView()
        {
            master = new TxtpaymentMaster();
            lstMaster = new List<TxtpaymentMaster>();
            lstDetailPurchase = new List<TxtpaymentDetail>();
            lstDetailOther = new List<TxtpaymentDetail>();
            lstLedger = new List<Txtledger>();
            lstCredit = new List<Txscoadetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstSupplier = new List<Txscoadetail>();
            lstExcise = new List<TxstaxDetail>();
            lstTax = new List<TxstaxDetail>();
            lstAccount = new List<Txscoadetail>();
        }
    }
}
