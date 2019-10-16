using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTPurchaseDetailView
    {
        public TxtpurchaseMaster master { get; set; }
        public List<TxtpurchaseMaster> lstMaster { get; set; }
        public TXTPurchaseDetailLstView detail { get; set; }
        public List<TxsbankDetail> lstBank { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public List<TxssupplierDetail> lstSupplier { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }
        public Txtledger ledger { get; set; }
        public TxtinventoryStockDetail inventory { get; set; }
        public double? SubTotal { get; set; }
        public double? ExciseTax { get; set; }
        public double? VAT { get; set; }
        public double? TotalAmount { get; set; }
        public double? Advance { get; set; }
        public double? Paid { get; set; }
        public double? TotalBalance { get; set; }


        public TXTPurchaseDetailView()
        {
            master = new TxtpurchaseMaster();
            lstMaster = new List<TxtpurchaseMaster>();
            detail = new TXTPurchaseDetailLstView();
            lstBank = new List<TxsbankDetail>();
            lstAccount = new List<Txscoadetail>();
            lstSupplier = new List<TxssupplierDetail>();
            lstStore = new List<TxsstoreDetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstSite = new List<TxssiteDetail>();
            lstItem = new List<TxsitemDetail>();
            lstTax = new List<TxstaxDetail>();
            lstExcise = new List<TxstaxDetail>();
            ledger = new Txtledger();
            inventory = new TxtinventoryStockDetail();
            SubTotal = new double();
            ExciseTax = new double();
            VAT = new double();
            TotalAmount = new double();
            Advance = new double();
            Paid = new double();
            TotalBalance = new double();
        }
    }
}