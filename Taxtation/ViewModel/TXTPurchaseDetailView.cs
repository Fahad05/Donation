using System;
using System.Collections.Generic;
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
        public List<TxssupplierDetail> lstSupplier { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }


        public TXTPurchaseDetailView()
        {
            master = new TxtpurchaseMaster();
            lstMaster = new List<TxtpurchaseMaster>();
            detail = new TXTPurchaseDetailLstView();
            lstBank = new List<TxsbankDetail>();
            lstSupplier = new List<TxssupplierDetail>();
            lstStore = new List<TxsstoreDetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstSite = new List<TxssiteDetail>();
            lstItem = new List<TxsitemDetail>();
            lstTax = new List<TxstaxDetail>();
            lstExcise = new List<TxstaxDetail>();
        }
    }
}