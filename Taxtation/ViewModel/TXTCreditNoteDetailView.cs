using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTCreditNoteDetailView
    {

        public TxtcreditNoteMaster master { get; set; }
        public List<TxtcreditNoteMaster> lstMaster { get; set; }
        public TXTCreditNoteDetailListView detail { get; set; }
        public List<TxscustomerDetail> lstCustomer { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }
        public List<TxsitemDetail> lstItem { get; set; }
        public List<TxssiteDetail> lstSite { get; set; }
        public double? SubTotal { get; set; }
        public double? ExciseTax { get; set; }
        public double? DiscountAmount { get; set; }
        public double? VAT { get; set; }
        public double? TotalAmount { get; set; }
        public double? Quantity { get; set; }


        public TXTCreditNoteDetailView()
        {

            master = new TxtcreditNoteMaster();
            lstMaster = new List<TxtcreditNoteMaster>();
            detail = new TXTCreditNoteDetailListView();
            lstCustomer = new List<TxscustomerDetail>();
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

    public class TXTCreditNoteDetailExtra
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
    public class TXTCreditNoteDetailListView
    {
        public List<TxtcreditNoteDetail> detail { get; set; }
        public List<TXTCreditNoteDetailExtra> cndef { get; set; }

        public TXTCreditNoteDetailListView()
        {
            detail = new List<TxtcreditNoteDetail>();
            cndef = new List<TXTCreditNoteDetailExtra>();
        }
    }


}
