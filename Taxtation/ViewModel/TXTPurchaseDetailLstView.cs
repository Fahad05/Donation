using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTPurchaseDetailLstView
    {
        public List<TxtpurchaseDetail> detail { get; set; }
        public List<PDEF> pdef { get; set; }

        public TXTPurchaseDetailLstView()
        {
            detail = new List<TxtpurchaseDetail>();
            pdef = new List<PDEF>();
        }
    }


    public class PDEF
    {
        public string UOM { get; set; }
        public double? lastPrice { get; set; }
        public double? subAmount { get; set; }
        public double? AmtAfterExcise { get; set; }
        public string DiscountType { get; set; }
        public double? AmtAfterDiscount { get; set; }
        public double? NetAmount { get; set; }

    }
}