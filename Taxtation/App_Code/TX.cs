using System;
using System.Linq;
using Taxtation.Models;


namespace Taxtation.App_Code
{
    public class TX
    {
        TAXTATIONContext db = new TAXTATIONContext();

        

        public string PurchaseOrder(string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtpurchaseMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.PurPoref).FirstOrDefault().PurPoref;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "PM-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "PM-00000001";
            }

            return temp;
        }



        public string SaleOrder(string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtsaleMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.SalSoRef).FirstOrDefault().SalSoRef;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "SM-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "SM-00000001";
            }

            return temp;
        }
    }
}
