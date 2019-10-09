using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSTaxDetailView
    {
        public TxstaxDetail tax { get; set; }
        public List<Txscoadetail> lstAccount { get; set; }

        public TXSTaxDetailView()
        {
            tax = new TxstaxDetail();
            lstAccount = new List<Txscoadetail>();
        }
    }
}
