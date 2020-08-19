using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSCustomerDetailView
    {
        public TxscustomerDetail master { get;set; }
        public List<TxscountryDetail> lstCountry { get; set; }
        public List<TxscityDetail> lstCity { get; set; }


        public TXSCustomerDetailView()
        {
            master = new TxscustomerDetail();
            lstCountry = new List<TxscountryDetail>();
            lstCity = new List<TxscityDetail>();
        }

    }
}
