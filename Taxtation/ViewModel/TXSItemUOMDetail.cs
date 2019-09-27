using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSItemUOMDetail
    {
        public TxsitemDetail Txsitem { get; set; }
        public Txsuomdetail Txuom { get; set; }

        public TXSItemUOMDetail()
        {
            Txsitem = new TxsitemDetail();
            Txuom = new Txsuomdetail();
        }
    }
}
