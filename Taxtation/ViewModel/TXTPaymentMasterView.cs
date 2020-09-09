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
        public List<TxtpaymentDetail> lstDetail { get; set; }
        public List<TxtpaymentBillDetail> lstDetailBill { get; set; }
        public List<TxtpaymentBillDetail> lstDetailBillCheck { get; set; }
        public List<Txscoadetail> lstCredit { get; set; }
        public List<Txtledger> lstLedger { get; set; }
        public List<Txtledger> lstDetailMultiple { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<TxssupplierDetail> lstSupplier { get; set; }
        public List<TxstaxDetail> lstAllTax { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public List<Txscoadetail> lstTaxAccount { get; set; }
        public List<Txscoadetail> lstAccountMultiple { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public TXTPaymentMasterView()
        {
            master = new TxtpaymentMaster();
            lstMaster = new List<TxtpaymentMaster>();
            lstDetail = new List<TxtpaymentDetail>();
            lstDetailBill = new List<TxtpaymentBillDetail>();
            lstDetailBillCheck = new List<TxtpaymentBillDetail>();
            lstLedger = new List<Txtledger>();
            lstDetailMultiple = new List<Txtledger>();
            lstCredit = new List<Txscoadetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstSupplier = new List<TxssupplierDetail>();
            lstAllTax = new List<TxstaxDetail>();
            lstExcise = new List<TxstaxDetail>();
            lstTax = new List<TxstaxDetail>();
            lstAccount = new List<Txscoadetail>();
            lstTaxAccount = new List<Txscoadetail>();
            lstAccountMultiple = new List<Txscoadetail>();
            lstSite = new List<TxssiteDetail>();
            lstItem = new List<TxsitemDetail>();
        }
    }
}
