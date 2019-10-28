using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTSaleDetailLstView
    {
        public List<TxtsaleDetail> saleDetail { get; set; }
        public List<saleDetail> sale { get; set; }


        public TXTSaleDetailLstView()
        {
            saleDetail = new List<TxtsaleDetail>();
            sale = new List<saleDetail>();
        }

    }


    public class saleDetail
    {
        public string BarCode { get; set; }
        public string UOM { get; set; }
        public double? LastPrice { get; set; }
        public double? StockQuantity { get; set; }
        public double? Amount { get; set; }
        public double? AmtAfterDiscount { get; set; }
        public double? AmtAfterExcise { get; set; }
    }
}
