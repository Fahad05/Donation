using System;
using System.Linq;
using Taxtation.Models;


namespace Taxtation.App_Code
{
    public class TX
    {
        TAXTATIONContext db = new TAXTATIONContext();
        /*
         * Scaffold-DbContext "Data Source=QaiserPC;Initial Catalog=TAXTATION;Persist Security Info=True;User ID=sa;Password=Sirsyed45;"  Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force
         */
        

        public string PurchaseOrder(string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtpurchaseMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.PurPoref).FirstOrDefault().PurPoref;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "PO-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "PO-00000001";
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
                    temp = "CN-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "CN-00000001";
            }
            return temp;
        }

        public string DebitNote(string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtdebitNoteMaster.Where(x => x.UserName == userName).OrderByDescending(x => x.PdnRefNo).FirstOrDefault().PdnRefNo;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "DN-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "DN-00000001";
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
                    temp = "SO-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "SO-00000001";
            }
            return temp;
        }

        public string JournalVoucher(string id,string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtjournalMaster.Where(x => x.Id==id && x.UserName == userName).OrderByDescending(x => x.Trno).FirstOrDefault().Trno;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "JV-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "JV-00000001";
            }
            return temp;
        }

        public string FundTransferVoucher(string id, string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtfundTransferMaster.Where(x => x.Id == id && x.UserName == userName).OrderByDescending(x => x.Trno).FirstOrDefault().Trno;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "FT-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "FT-00000001";
            }
            return temp;
        }
        public string OpeningDetail(string id, string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtopeningMaster.Where(x => x.Id == id && x.UserName == userName).OrderByDescending(x => x.Trno).FirstOrDefault().Trno;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "OP-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "OP-00000001";
            }
            return temp;
        }
        public string PaymentVoucher(string id, string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtpaymentMaster.Where(x => x.Id == id && x.UserName == userName).OrderByDescending(x => x.Trno).FirstOrDefault().Trno;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "PV-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "PV-00000001";
            }
            return temp;            
        }
        public string ReceiptVoucher(string id, string userName)
        {
            string temp = "";
            try
            {
                var max = db.TxtreceiptMaster.Where(x => x.Id == id && x.UserName == userName).OrderByDescending(x => x.Trno).FirstOrDefault().Trno;
                string subString = max.Substring(3, 8);
                if (!string.IsNullOrEmpty(max))
                {
                    temp = "RV-" + (Convert.ToInt32(subString) + 1).ToString("D8");
                }
            }
            catch (Exception ex)
            {
                temp = "RV-00000001";
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

        #region Insert

        public void insertLedgerDetail(string id, string userName, string Trno, DateTime? Trdate, DateTime? Trgldate, string Trtype,
            string TrpostStatus, double TrserialNo, string TraccCode, string TrrefAccCode, string TrdocReference,
            double? Trdebit, double? Trcredit, double? TrexchangeRate, double? TramountConverted, int? TrcurId, int? TrtxsId,
            double? TrtaxPercent, double? TrtaxAmount, string TrchequeNo, DateTime? TrchequeDate, string TrrefNo, string Trremarks, string TrentryType,
            string EnterBy, DateTime? EnterDate, string EntryFrom, int? SitId, string TrentryTypeDoc,
            int? TRCustomerRef, int? TRSupplierRef, int? TRTaxRef)
        {
            Txtledger obj = new Txtledger();
            obj.Id = id;
            obj.UserName = userName;
            obj.Trno = Trno;
            obj.Trdate = Trdate;
            obj.Trgldate = Trgldate;
            obj.Trtype = Trtype;
            obj.TrpostStatus = TrpostStatus;
            obj.TrserialNo = TrserialNo;
            obj.TraccCode = TraccCode;
            obj.TrrefAccCode = TrrefAccCode;
            obj.TrdocReference = TrdocReference;
            obj.Trdebit = Trdebit;
            obj.Trcredit = Trcredit;
            obj.TrexchangeRate = TrexchangeRate;
            obj.TramountConverted = TramountConverted;
            obj.TrcurId = TrcurId;
            obj.TrtxsId = TrtxsId;
            obj.TrtaxPercent = TrtaxPercent;
            obj.TrtaxAmount = TrtaxAmount;
            obj.TrchequeNo = TrchequeNo;
            obj.TrchequeDate = TrchequeDate;
            obj.TrrefNo = TrrefNo;
            obj.Trremarks = Trremarks;
            obj.TrentryType = TrentryType;
            obj.EnterBy = EnterBy;
            obj.EnterDate = EnterDate;
            obj.EntryFrom = EntryFrom;
            obj.SitId = SitId;
            obj.TrentryTypeDoc = TrentryTypeDoc;
            obj.TrcustomerRef = TRCustomerRef;
            obj.TrsupplierRef = TRSupplierRef;
            obj.TrtaxRef = TRTaxRef;
            db.Txtledger.Add(obj);
            db.SaveChanges();
        }

        public void insertLedgerPaymentDetail(string id, string userName, string Trno, DateTime? Trdate, DateTime? Trgldate, string Trtype,
            string TrpostStatus, double TrserialNo, string TraccCode, string TrrefAccCode, string TrdocReference,
            double? Trdebit, double? Trcredit, double? TrexchangeRate, double? TramountConverted, int? TrcurId, int? TrtxsId,
            double? TrtaxPercent, double? TrtaxAmount, string TrchequeNo, DateTime? TrchequeDate, string TrrefNo, string Trremarks, string TrentryType,
            string EnterBy, DateTime? EnterDate, string EntryFrom, int? SitId, string TrentryTypeDoc,
            int? TRCustomerRef, int? TRSupplierRef, int? TRTaxRef, double? Tramount, double? TramountWithTax, int? TrtxsExciseId, double? TrtaxExcisePercent, double? TrtaxExciseAmount)
        {
            Txtledger obj = new Txtledger();
            obj.Id = id;
            obj.UserName = userName;
            obj.Trno = Trno;
            obj.Trdate = Trdate;
            obj.Trgldate = Trgldate;
            obj.Trtype = Trtype;
            obj.TrpostStatus = TrpostStatus;
            obj.TrserialNo = TrserialNo;
            obj.TraccCode = TraccCode;
            obj.TrrefAccCode = TrrefAccCode;
            obj.TrdocReference = TrdocReference;
            obj.Trdebit = Trdebit;
            obj.Trcredit = Trcredit;
            obj.TrexchangeRate = TrexchangeRate;
            obj.TramountConverted = TramountConverted;
            obj.TrcurId = TrcurId;
            obj.TrtxsId = TrtxsId;
            obj.TrtaxPercent = TrtaxPercent;
            obj.TrtaxAmount = TrtaxAmount;
            obj.TrchequeNo = TrchequeNo;
            obj.TrchequeDate = TrchequeDate;
            obj.TrrefNo = TrrefNo;
            obj.Trremarks = Trremarks;
            obj.TrentryType = TrentryType;
            obj.EnterBy = EnterBy;
            obj.EnterDate = EnterDate;
            obj.EntryFrom = EntryFrom;
            obj.SitId = SitId;
            obj.TrentryTypeDoc = TrentryTypeDoc;
            obj.TrcustomerRef = TRCustomerRef;
            obj.TrsupplierRef = TRSupplierRef;
            obj.TrtaxRef = TRTaxRef;

            obj.Tramount = Tramount;
            obj.TramountWithTax = TramountWithTax;
            obj.TrtxsExciseId = TrtxsExciseId;
            obj.TrtaxExcisePercent = TrtaxExcisePercent;
            obj.TrtaxExciseAmount = TrtaxExciseAmount;

            db.Txtledger.Add(obj);
            db.SaveChanges();
        }

        public void insertPaymentBill(string id, string userName, string PblTrcode, string PblCode, DateTime? PblDate, double? PblSerialNo, string InvCode, double? PblBillAmount, double? PblOwingAmount, double? PblPaidAmount, double? PblBalanceAmount, string PblSubRemarks, string PblSupCode, int? SupId)
        {
            TxtpaymentBillDetail pBill = new TxtpaymentBillDetail();
            pBill.Id = id;
            pBill.UserName = userName;
            pBill.PblTrcode = PblTrcode;
            pBill.PblCode = PblCode;
            pBill.PblDate = PblDate;
            pBill.PblSerialNo = PblSerialNo;
            pBill.InvCode = InvCode;
            pBill.PblBillAmount = PblBillAmount;
            pBill.PblOwingAmount = PblOwingAmount;
            pBill.PblPaidAmount = PblPaidAmount;
            pBill.PblBalanceAmount = PblBalanceAmount;
            pBill.PblSubRemarks = PblSubRemarks;
            pBill.PblSupCode = PblSupCode;
            pBill.SupId = SupId;
            db.TxtpaymentBillDetail.Add(pBill);
            db.SaveChanges();
        }

        public void insertInventoryStockDetail(string id, string userName, int? ItmId, double? InsSerialNo, string InsNo, DateTime? InsDate, string InsRefNo, string InsRemarks, string InsSubRemarks, double? InsQuantityIn, double? InsQuantityOut, double? InsWeightIn, double? InsWeightOut, int? StrId, int? TxsId, double? QprTaxPercent, double? QprAmount, int? CurId, double? QprExchangeRate, double? QprAmountConverted, string InsEntryType, string InsPostStatus)
        {
            TxtinventoryStockDetail inv = new TxtinventoryStockDetail();
            inv.Id = id;
            inv.UserName = userName;
            inv.ItmId = ItmId;
            inv.InsSerialNo = InsSerialNo;
            inv.InsNo = InsNo;
            inv.InsDate = InsDate;
            inv.InsRefNo = InsRefNo;
            inv.InsRemarks = InsRemarks;
            inv.InsSubRemarks = InsSubRemarks;
            inv.InsQuantityIn = InsQuantityIn;
            inv.InsQuantityOut = InsQuantityOut;
            inv.InsWeightIn = InsWeightIn;
            inv.InsWeightOut = InsWeightOut;
            inv.StrId = StrId;
            inv.TxsId = TxsId;
            inv.QprTaxPercent = QprTaxPercent;
            inv.QprAmount = QprAmount;
            inv.CurId = CurId;
            inv.QprExchangeRate = QprExchangeRate;
            inv.QprAmountConverted = QprAmountConverted;
            inv.InsEntryType = InsEntryType;
            inv.InsPostStatus = InsPostStatus;
            db.TxtinventoryStockDetail.Add(inv);
            db.SaveChanges();
        }



        #endregion



    }
}
