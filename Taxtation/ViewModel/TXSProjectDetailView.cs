using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;
namespace Taxtation.ViewModel
{
    public class TXSProjectDetailView
    {
        public TxsprojectDetail master { get; set; }

        public List<TxsorganizationDetail> lstOrg { get; set; }

        public TXSProjectDetailView()
        {
            master = new TxsprojectDetail();

            lstOrg = new List<TxsorganizationDetail>();

        }
    }
}
