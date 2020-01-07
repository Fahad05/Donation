using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using Taxtation.App_Code;
using Taxtation.Models;
using Taxtation.Models.ManageViewModels;
using Taxtation.Services;
using Taxtation.ViewModel;

namespace Taxtation.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class GeneralLedgerController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        TAXTATIONContext db = new TAXTATIONContext();
        TX tX = new TX();
        public GeneralLedgerController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<TransactionController> logger,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        #region Journal Detail
        [HttpGet]
        public async Task<IActionResult> showJournalDetail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTJournalDetailView obj = new TXTJournalDetailView();
            obj.lstMaster= db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trtype == "JOURNAL" && x.TrentryType == "CHILD").ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true).ToList();
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> JournalDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTJournalDetailView obj = new TXTJournalDetailView();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccActive == true).ToList();
            obj.master.Tradjusting = (obj.master.Tradjusting == true) ? true : false;
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trno = tX.JournalVoucher(user.Id, user.UserName);
                obj.master.Trdate = DateTime.Now;
                obj.master.Trgldate = DateTime.Now;
                obj.master.TrexchangeRate = 1;
                obj.lstLedger = null;
                obj.eFJV = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno && x.TrentryType == "CHILD").OrderBy(x => x.TrserialNo).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    EFJV eFJV = new EFJV();
                    if (obj.lstLedger[i].Trdebit != 0) { eFJV.amount = obj.lstLedger[i].Trdebit; eFJV.debitCredit = "001"; }
                    if (obj.lstLedger[i].Trcredit != 0) { eFJV.amount = obj.lstLedger[i].Trcredit; eFJV.debitCredit = "002"; }
                    obj.eFJV.Add(eFJV);
                }
                obj.TotalCredit = obj.lstLedger.Sum(x => x.Trcredit);
                obj.TotalDebit = obj.lstLedger.Sum(x => x.Trdebit);
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> JournalDetail(TXTJournalDetailView obj, string Save, string Update, string Delete, string TraAdusting)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            if (Save != null)
            {
                obj.master.Id = user.Id;
                obj.master.UserName = user.UserName;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = DateTime.Now;
                obj.master.Tradjusting = (TraAdusting == "true") ? true : false;
                db.TxtjournalMaster.Add(obj.master);
                db.SaveChanges();
                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.eFJV[i].amount != null)
                    {
                        if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                        if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Update != null)
            {
                TxtjournalMaster objUpdate = new TxtjournalMaster();
                objUpdate = db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == obj.master.TrId).FirstOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.Trdate = obj.master.Trdate;
                    objUpdate.Trgldate = obj.master.Trgldate;
                    objUpdate.SitId = obj.master.SitId;
                    objUpdate.TrtotalAmount = obj.master.TrtotalAmount;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.TrcurId = obj.master.TrcurId;
                    objUpdate.Tradjusting = (TraAdusting == "true") ? true : false;
                    objUpdate.TrrefMain = obj.master.TrrefMain;
                    objUpdate.EditBy = user.UserName;
                    objUpdate.EditDate = DateTime.Now;
                    db.SaveChanges();
                }


                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    db.Txtledger.Remove(obj.lstLedger[i]);
                    db.SaveChanges();
                }

                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.eFJV[i].amount != null)
                    {
                        if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                        if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Delete != null)
            {
                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    db.Txtledger.Remove(obj.lstLedger[i]);
                    db.SaveChanges();
                }
                obj.master = db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                db.TxtjournalMaster.Remove(obj.master);
                db.SaveChanges();
            }
            return RedirectToAction("showJournalDetail");
        }

        #endregion


        #region Payment Detail
        [HttpGet]
        public IActionResult showPayment()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PaymentDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTPaymentMasterView obj = new TXTPaymentMasterView();
            obj.lstCredit = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountSubNature.Contains("BANK ACCOUNT")).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstSupplier = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName || x.AccAccountSubNature.Contains("SUPPLIER") || x.AccAccountSubNature.Contains("CONTRACTOR")).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.AccAccountSubNature != "TAX" && (x.AccAccountNature == "ASSET" || x.AccAccountNature == "EXPENSE" || x.AccAccountNature == "LIABILITY")).OrderBy(x => x.AccName).ToList();
            obj.lstExcise = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxCategory == "EXCISE").ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxCategory == "VAT").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trdate = DateTime.Now;
                obj.master.Trno = tX.PaymentVoucher(user.Id, user.UserName);
                obj.lstDetailPurchase = null;
                obj.lstDetailOther = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";

            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult PaymentDetail(TXTPaymentMasterView obj, string Save, string Update, string Delete)
        {
            return View();
        }

        #endregion


        #region Functions

        public List<Txscoadetail> PaymentCredit(string BankCash)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            List<Txscoadetail> lstCredit = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature == BankCash).ToList();
            return lstCredit;
        }


        public TXTPaymentMasterView supplierChange(string Trno , int ExchangeRate, string supplier)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            TXTPaymentMasterView query = new TXTPaymentMasterView();
            List<TxtpurchaseMaster> purchase = new List<TxtpurchaseMaster>();
            int supId = accCodeToSupplier(supplier);
            Txtledger Ledger = new Txtledger();
            purchase = db.TxtpurchaseMaster.Where(x => x.Id == id && x.UserName == userName && x.SupId == supId && x.PurPayTerm== "CREDIT").ToList();
            if(purchase != null)
            {
                for (int i = 0; i < purchase.Count; i++)
                {
                    string TotalPaid = db.Txtledger.Where(x => x.Id == id && x.UserName == userName && x.Trno != Trno && x.TrrefNo == purchase[i].PurPoref && x.TrentryTypeDoc == "CHILD").Sum(x => x.Trdebit - x.Trcredit).ToString();
                    if (TotalPaid != null)
                    {
                        TxtpaymentDetail payment = new TxtpaymentDetail();
                        payment.PayBillDate = purchase[i].PurDate;
                        payment.PayBillNo = purchase[i].PurPoref;
                        payment.PayOriginalAmt = Convert.ToDouble(db.TxtpurchaseDetail.Where(x => x.Id == id && x.UserName == userName && x.PurId == purchase[i].PurId).Sum(x => x.PurGrossAmt).ToString());
                        payment.PayAmtOwing = payment.PayOriginalAmt - Convert.ToDouble(TotalPaid);
                        payment.PayDiscAmt = Convert.ToDouble(db.TxtpurchaseDetail.Where(x => x.Id == id && x.UserName == userName && x.PurId == purchase[i].PurId).Sum(x => x.PurDiscountAmt).ToString());
                        payment.PayPaymentAmt = payment.PayOriginalAmt - Convert.ToDouble(TotalPaid);
                        payment.PayExcAmt = (payment.PayOriginalAmt - Convert.ToDouble(TotalPaid)) * ExchangeRate;
                        query.lstDetailOther.Add(payment);
                    }
                }
            }
            return query;
        }


        #endregion




        public int accCodeToSupplier(string SupCode)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            var CoaId =  db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccCode == SupCode).FirstOrDefault().Coaid;
            return db.TxssupplierDetail.Where(x => x.Id == id && x.UserName == userName && x.CoaId == CoaId).FirstOrDefault().SupId;
        }

    }



}
