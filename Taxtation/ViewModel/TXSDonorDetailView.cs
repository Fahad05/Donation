using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSDonorDetailView
    {
        public TxsdonorDetail master { get; set; }
        public List<TxscountryDetail> lstCountry { get; set; }
        public List<TxscityDetail> lstCity { get; set; }

        public TXSDonorDetailView()
        {
            master = new TxsdonorDetail();
            lstCountry = new List<TxscountryDetail>();
            lstCity = new List<TxscityDetail>();
            
        }
    }
}
