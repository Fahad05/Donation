using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTSaleDetailView
    {
        public TxtsaleMaster master { get; set; }
        public List<TxtsaleMaster> lstMaster { get; set; }
        public List<TxtsaleDetail> lstDetails { get; set; }
        public TXTSaleDetailLstView Detail { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }
        public List<TxscustomerDetail> lstCustomer { get; set; }
        public List<TxscurrencyDetail> lstCurrency { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public List<TxstaxDetail> lstTax { get; set; }
        public List<TxstaxDetail> lstExcise { get; set; }
        public List<TxsbankDetail> lstBank { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }
        public double? SubTotal { get; set; }
        public double? ExciseTax { get; set; }
        public double? VAT { get; set; }
        public double? TotalAmount { get; set; }
        public double? Advance { get; set; }
        public double? Paid { get; set; }
        public double? TotalBalance { get; set; }

        public TXTSaleDetailView()
        {
            master = new TxtsaleMaster();
            lstMaster = new List<TxtsaleMaster>();
            lstDetails = new List<TxtsaleDetail>();
            Detail = new TXTSaleDetailLstView();
            lstStore = new List<TxsstoreDetail>();
            lstCustomer = new List<TxscustomerDetail>();
            lstCurrency = new List<TxscurrencyDetail>();
            lstSite = new List<TxssiteDetail>();
            lstItem = new List<TxsitemDetail>();
            lstTax = new List<TxstaxDetail>();
            lstExcise = new List<TxstaxDetail>();
            lstBank = new List<TxsbankDetail>();
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
