using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;
namespace Taxtation.ViewModel
{
    public class TXSOrganizationDetailView
    {
        public TxsorganizationDetail master { get; set; }
        public List<TxscountryDetail> lstCountry { get; set; }
        public List<TxscityDetail> lstCity { get; set; }

        public TXSOrganizationDetailView()
        {
            master = new TxsorganizationDetail();
            lstCountry = new List<TxscountryDetail>();
            lstCity = new List<TxscityDetail>();

        }
    }
}
