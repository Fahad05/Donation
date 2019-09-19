using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSItemDetailView
    {
        public TxsitemDetail master { get; set; }
        public List<TxsitemDetail> lstParent { get; set; }
        public List<Txsuomdetail> lstUOM { get; set; }
        public List<TxsstoreDetail> lstStore { get; set; }

        public TXSItemDetailView()
        {
            master = new TxsitemDetail();
            lstParent = new List<TxsitemDetail>();
            lstUOM = new List<Txsuomdetail>();
            lstStore = new List<TxsstoreDetail>();
        }

    }
}
