using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTDebitNoteDetailView
    {
        public TxtdebitNoteMaster master { get; set; }
        public List<TxtdebitNoteMaster> lstMaster { get; set; }
        public TXTDebitNoteDetailListView detail { get; set; }
        public List<TxssupplierDetail> lstSupplier { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public double? SubTotal { get; set; }
        public double? ExciseTax { get; set; }
        public double? DiscountAmount { get; set; }
        public double? VAT { get; set; }
        public double? TotalAmount { get; set; }
        public double? Quantity { get; set; }


        public TXTDebitNoteDetailView()
        {

            master = new TxtdebitNoteMaster();
            lstMaster = new List<TxtdebitNoteMaster>();
            detail = new TXTDebitNoteDetailListView();
            lstSupplier = new List<TxssupplierDetail>();
            lstStore = new List<TxsstoreDetail>();
            lstItem = new List<TxsitemDetail>();
            lstSite = new List<TxssiteDetail>();
            SubTotal = new double();
            ExciseTax = new double();
            DiscountAmount = new double();
            VAT = new double();
            TotalAmount = new double();
            Quantity = new double();
        }
    }
    public class TXTDebitNoteDetailExtra
    {
        public string UOM { get; set; }
        public double? lastPrice { get; set; }
        public double? subAmount { get; set; }
        public double? BalAmount { get; set; }
        public double? AmtAfterExcise { get; set; }
        public string DiscountType { get; set; }
        public double? AmtAfterDiscount { get; set; }
        public double? NetAmount { get; set; }

    }
    public class TXTDebitNoteDetailListView
    {
        public List<TxtdebitNoteDetail> detail { get; set; }
        public List<TXTDebitNoteDetailExtra> dndef { get; set; }

        public TXTDebitNoteDetailListView()
        {
            detail = new List<TxtdebitNoteDetail>();
            dndef = new List<TXTDebitNoteDetailExtra>();
        }
    }
}
