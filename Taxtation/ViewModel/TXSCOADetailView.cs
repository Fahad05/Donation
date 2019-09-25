using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXSCOADetailView
    {
        public Txscoadetail coa { get; set; }
        public List<Txscoadetail> lstCoa { get; set; }

        public TXSCOADetailView()
        {
            coa = new Txscoadetail();
            lstCoa = new List<Txscoadetail>();
        }
    }
}
