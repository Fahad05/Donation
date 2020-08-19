using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxtation.Models;

namespace Taxtation.App_Code
{
    public class TX
    {
        TAXTATIONContext db = new TAXTATIONContext();

        [Route("TX/lstCity")]
        public List<TxscityDetail> lstCity(string CouCode)
        {
            List<TxscityDetail> lst = new List<TxscityDetail>();
            lst = db.TxscityDetail.Where(x => x.CouCode == CouCode).ToList();
            return lst;
        }
    }
}
