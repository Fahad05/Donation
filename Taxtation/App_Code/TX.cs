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
        public string CreditNote(string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtcreditNoteMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.ScnRefNo).FirstOrDefault().ScnRefNo;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "PM-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "CN-00000001";
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


        public string getParentAccountCodeDetail(string id)
        {
            return db.Txscoadetail.Where(x => x.AccParentAccount == id).Max(x => x.AccCode);
        }

        public Txscoadetail findParentAccount(string accCode)
        {
            return db.Txscoadetail.Where(x => x.AccParentAccount == accCode).FirstOrDefault();
        }

        //public TxtpurchaseMaster findParentAccountPaymentMaster(string accCode)
        //{
        //    return db.TxtpurchaseMaster.Where(x => x.TrtypeAccount == accCode).FirstOrDefault();
        //}

        //public GltreceiptMaster findParentAccountReceiptMaster(string accCode)
        //{
        //    return db.GltreceiptMaster.Where(x => x.TrtypeAccount == accCode).FirstOrDefault();
        //}
    }
}
