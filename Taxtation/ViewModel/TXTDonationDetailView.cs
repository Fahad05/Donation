using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.ViewModel
{
    public class TXTDonationDetailView
    {
        public TxtdonationDetail master { get; set; }

        public List<TxsdonorDetail> lstDonor { get; set; }

        public List<TxsdonationCategoryDetail> lstCategory { get; set; }

        public TXTDonationDetailView()
        {
            master = new TxtdonationDetail();
            lstDonor = new List<TxsdonorDetail>();
            lstCategory = new List<TxsdonationCategoryDetail>();
        }
    }
}
