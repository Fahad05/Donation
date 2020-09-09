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

        #region Opening Detail
        [HttpGet]
        public async Task<IActionResult> showOpeningDetail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTOpeningDetailView obj = new TXTOpeningDetailView();
            obj.lstMaster = db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trtype == "OPENING" && x.TrentryType == "CHILD").ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            //obj.lstSite.Select(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();

            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true).ToList();
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> OpeningDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTOpeningDetailView obj = new TXTOpeningDetailView();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccActive == true).ToList();

            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trno = tX.OpeningDetail(user.Id, user.UserName);
                obj.master.Trdate = DateTime.Now;
                obj.master.Trgldate = DateTime.Now;
                obj.master.TrexchangeRate = 1;
                obj.lstLedger = null;
                obj.eFOD = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno && x.TrentryType == "CHILD").OrderBy(x => x.TrserialNo).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    EFOD eFOD = new EFOD();
                    if (obj.lstLedger[i].Trdebit != 0) { eFOD.amount = obj.lstLedger[i].Trdebit; eFOD.debitCredit = "001"; }
                    if (obj.lstLedger[i].Trcredit != 0) { eFOD.amount = obj.lstLedger[i].Trcredit; eFOD.debitCredit = "002"; }
                    obj.eFOD.Add(eFOD);
                }
                obj.TotalCredit = obj.lstLedger.Sum(x => x.Trcredit);
                obj.TotalDebit = obj.lstLedger.Sum(x => x.Trdebit);
            }
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> OpeningDetail(TXTOpeningDetailView obj, string Save, string Update, string Delete, string TraAdusting)
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
                obj.master.Trno = tX.OpeningDetail(user.Id, user.UserName);
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = DateTime.Now;
                obj.master.TrtotalAmount = obj.TotalCredit;
                db.TxtopeningMaster.Add(obj.master);
                db.SaveChanges();
                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.eFOD[i].amount != null)
                    {
                        if (obj.eFOD[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFOD[i].amount; }
                        if (obj.eFOD[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFOD[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "OPENING", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Opening Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        //k = k + 1;
                        //tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "OPENING", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Opening Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Update != null)
            {
                TxtopeningMaster objUpdate = new TxtopeningMaster();
                objUpdate = db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == obj.master.TrId).FirstOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.Trdate = obj.master.Trdate;
                    objUpdate.Trgldate = obj.master.Trgldate;
                    objUpdate.SitId = obj.master.SitId;
                    objUpdate.TrtotalAmount = obj.TotalCredit;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.TrcurId = obj.master.TrcurId;
                    objUpdate.TrrefMain = obj.master.TrrefMain;
                    objUpdate.EditBy = user.UserName;
                    objUpdate.EditDate = DateTime.Now;
                    db.SaveChanges();
                }


                //obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                //for (int i = 0; i < obj.lstLedger.Count; i++)
                //{
                //    db.Txtledger.Remove(obj.lstLedger[i]);
                //    db.SaveChanges();
                //}

                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                int k = 0;

                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.eFOD[i].amount != null)
                    {
                        if (obj.eFOD[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFOD[i].amount; }
                        if (obj.eFOD[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFOD[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "OPENING", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Opening Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        //k = k + 1;
                        //tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "OPENING", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Opening Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Delete != null)
            {
                //obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                //for (int i = 0; i < obj.lstLedger.Count; i++)
                //{
                //    db.Txtledger.Remove(obj.lstLedger[i]);
                //    db.SaveChanges();
                //}

                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                //obj.master = db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                //db.TxtopeningMaster.Remove(obj.master);
                //db.SaveChanges();

                db.TxtopeningMaster.RemoveRange(db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();
            }
            return RedirectToAction("showOpeningDetail");
        }

        #endregion

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
                //obj.TotalCredit = obj.master.TrtotalAmount;
                //obj.TotalDebit = obj.master.TrtotalAmount;
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
                obj.master.Trno = tX.JournalVoucher(user.Id, user.UserName);
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = DateTime.Now;
                obj.master.Tradjusting = (TraAdusting == "true") ? true : false;
                obj.master.TrtotalAmount = obj.TotalCredit;
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
                        //k = k + 1;
                        //tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "PARENT", 0, 0, 0);
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
                    objUpdate.TrtotalAmount = obj.TotalCredit;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.TrcurId = obj.master.TrcurId;
                    objUpdate.Tradjusting = (TraAdusting == "true") ? true : false;
                    objUpdate.TrrefMain = obj.master.TrrefMain;
                    objUpdate.EditBy = user.UserName;
                    objUpdate.EditDate = DateTime.Now;
                    //objUpdate.TrtotalAmount = obj.TotalCredit;
                    db.SaveChanges();
                }


                //obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                //for (int i = 0; i < obj.lstLedger.Count; i++)
                //{
                //    db.Txtledger.Remove(obj.lstLedger[i]);
                //    db.SaveChanges();
                //}

                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
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
                        //k = k + 1;
                        //tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "JOURNAL", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Journal Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Delete != null)
            {

                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                //obj.master = db.TxtopeningMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                //db.TxtopeningMaster.Remove(obj.master);
                //db.SaveChanges();

                db.TxtjournalMaster.RemoveRange(db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                //obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).ToList();
                //for (int i = 0; i < obj.lstLedger.Count; i++)
                //{
                //    db.Txtledger.Remove(obj.lstLedger[i]);
                //    db.SaveChanges();
                //}
                //obj.master = db.TxtjournalMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                //db.TxtjournalMaster.Remove(obj.master);
                //db.SaveChanges();
            }
            return RedirectToAction("showJournalDetail");
        }

        #endregion

        #region Fund Transfer Deatail
        public async Task<IActionResult> showFundTransferDetail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTFundTransferDetailView obj = new TXTFundTransferDetailView();
            obj.lstMaster = db.TxtfundTransferMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trtype == "FUND TRANSFER" && x.TrentryType == "CHILD").ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true).ToList();
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> FundTransferDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTFundTransferDetailView obj = new TXTFundTransferDetailView();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccActive == true).ToList();
          
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trno = tX.FundTransferVoucher(user.Id, user.UserName);
                obj.master.Trdate = DateTime.Now;
                obj.master.Trgldate = DateTime.Now;
                obj.master.TrexchangeRate = 1;
                obj.lstLedger = null;
                
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtfundTransferMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno && x.TrentryType == "CHILD").OrderBy(x => x.TrserialNo).ToList();
                obj.TotalCredit = obj.lstLedger.Sum(x => x.Trcredit);
                obj.TotalDebit = obj.lstLedger.Sum(x => x.Trdebit);
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> FundTransferDetail(TXTFundTransferDetailView obj, string Save, string Update, string Delete, string TraAdusting)
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
                db.TxtfundTransferMaster.Add(obj.master);
                db.SaveChanges();
                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" /*&& obj.eFJV[i].amount != null*/)
                    {
                     //   if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                      //  if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "FUND TRANSFER", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Fund Trnasfer Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "FUND TRANSFER", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Fund Transfer Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Update != null)
            {
                TxtfundTransferMaster objUpdate = new TxtfundTransferMaster();
                objUpdate = db.TxtfundTransferMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TrId == obj.master.TrId).FirstOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.Trdate = obj.master.Trdate;
                    objUpdate.Trgldate = obj.master.Trgldate;
                    objUpdate.SitId = obj.master.SitId;
                    objUpdate.TrtotalAmount = obj.master.TrtotalAmount;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.TrcurId = obj.master.TrcurId;
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
                    if (obj.lstLedger[i].TraccCode != "-1" /*&& obj.eFJV[i].amount != null*/)
                    {
                        //if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                        //if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "FUND TRANSFER", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Fund Transfer Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "FUND TRANSFER", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.TrcurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Fund Transfer Detail", obj.master.SitId, "PARENT", 0, 0, 0);
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
                obj.master = db.TxtfundTransferMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                db.TxtfundTransferMaster.Remove(obj.master);
                db.SaveChanges();
            }
            return RedirectToAction("showFundTransferDetail");
        }
        #endregion

        #region Receipt Detail
        public IActionResult showReceipt()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ReceiptDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTReceiptMasterView obj = new TXTReceiptMasterView();
            obj.lstDebit = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountSubNature.Contains("BANK ACCOUNT")).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstCustomer = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName || x.AccAccountSubNature.Contains("CUSTOMER")).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.AccAccountSubNature != "TAX" && x.AccAccountSubNature != "BANK ACCOUNT" && x.AccAccountSubNature != "CASH" && (x.AccAccountNature == "ASSET" || x.AccAccountNature == "REVENUE")).OrderBy(x => x.AccName).ToList();
            obj.lstExcise = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "SALE" && x.TaxCategory == "EXCISE").ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "SALE" && x.TaxCategory == "VAT").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trdate = DateTime.Now;
                obj.master.Trno = tX.ReceiptVoucher(user.Id, user.UserName);
                obj.lstDetailSale = null;
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
        public async Task<IActionResult> ReceiptDetail(TXTReceiptMasterView obj, string Save, string Update, string Delete)
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
               // obj.master.Tradjusting = (TraAdusting == "true") ? true : false;

                db.TxtreceiptMaster.Add(obj.master);
                db.SaveChanges();
                int k = 0;
                for (int i = 0; i < obj.lstDetailSale.Count; i++)
                {
                    if (obj.lstDetailSale[i].RecChqNo != "" && obj.lstDetailSale[i].RecChqDate != null)
                    {
                       // if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                       // if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "RECEIPT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Receipt Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "RECEIPT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Receipt Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Update != null)
            {
                TxtreceiptMaster objUpdate = new TxtreceiptMaster();
                objUpdate = db.TxtreceiptMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Id == obj.master.Id).FirstOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.Trdate = obj.master.Trdate;
                    objUpdate.Trgldate = obj.master.Trgldate;
                    objUpdate.SitId = obj.master.SitId;
                    objUpdate.TrtotalAmount = obj.master.TrtotalAmount;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.Trtype = obj.master.Trtype;
                    objUpdate.TrtypeAccount  = obj.master.TrtypeAccount;
                    objUpdate.TrtypeAccountName = obj.master.TrtypeAccountName;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;

                    objUpdate.CurId = obj.master.CurId;
                    objUpdate.TrrefMain = obj.master.TrrefMain;
                    objUpdate.EditBy = user.UserName;
                    objUpdate.EditDate = DateTime.Now;
                    db.SaveChanges();
                }


                obj.lstDetailSale = db.TxtreceiptDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.RecId == obj.master.RecId).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    db.Txtledger.Remove(obj.lstLedger[i]);
                    db.SaveChanges();
                }

                int k = 0;
                for (int i = 0; i < obj.lstDetailSale.Count; i++)
                {
                    if (obj.lstDetailSale[i].RecChqNo != "" && obj.lstDetailSale[i].RecChqDate != null)
                    {
                      //  if (obj.eFJV[i].debitCredit == "001") { obj.lstLedger[i].Trcredit = 0; obj.lstLedger[i].Trdebit = obj.eFJV[i].amount; }
                      //  if (obj.eFJV[i].debitCredit == "002") { obj.lstLedger[i].Trcredit = obj.eFJV[i].amount; obj.lstLedger[i].Trdebit = 0; }
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "RECEIPT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Receipt Detail", obj.master.SitId, "CHILD", 0, 0, 0);
                        k = k + 1;
                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "RECEIPT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.lstLedger[i].TraccCode, "", obj.lstLedger[i].Trdebit, obj.lstLedger[i].Trcredit, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, 0, 0, 0, "", null, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "TOTAL", user.UserName, DateTime.Now, "Receipt Detail", obj.master.SitId, "PARENT", 0, 0, 0);
                    }
                }
            }
            if (Delete != null)
            {
                obj.lstDetailSale = db.TxtreceiptDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.RecId == obj.master.RecId).ToList();
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    db.TxtreceiptDetail.Remove(obj.lstDetailSale[i]);
                    db.SaveChanges();
                }
                obj.master = db.TxtreceiptMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                db.TxtreceiptMaster.Remove(obj.master);
                db.SaveChanges();
            }

            return RedirectToAction("showReceiptDetail");
          
        }
        #endregion

        #region Payment Detail
        [HttpGet]
        public async Task<IActionResult> showPayment()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTPaymentMasterView obj = new TXTPaymentMasterView();
            obj.lstMaster = db.TxtpaymentMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trtype == "PAYMENT" && x.TrentryType == "CHILD").ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurActive == true).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true).ToList();
            return View(obj);
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
            //obj.lstSupplier = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountSubNature.Contains("SUPPLIER")).ToList();
            obj.lstSupplier = db.TxssupplierDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.AccAccountSubNature != "TAX" && x.AccAccountSubNature != "BANK ACCOUNT" && x.AccAccountSubNature != "CASH" && (x.AccAccountNature == "ASSET" || x.AccAccountNature == "EXPENSE" || x.AccAccountNature == "LIABILITY")).OrderBy(x => x.AccName).ToList();
            obj.lstAccountMultiple = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && (x.AccAccountSubNature.Contains("BANK ACCOUNT") || x.AccAccountSubNature.Contains("CASH"))).ToList();
            obj.lstExcise = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxCategory == "EXCISE").ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxCategory == "VAT").ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitActive == true).ToList();

            //obj.lstDetailMultiple = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && (x.AccAccountSubNature.Contains("BANK ACCOUNT") || x.AccAccountSubNature.Contains("CASH"))).ToList();

            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.Trno = tX.PaymentVoucher(user.Id, user.UserName);
                obj.master.Trdate = DateTime.Now;
                obj.master.Trgldate = DateTime.Now;
                obj.master.TrexchangeRate = 1;
                obj.lstLedger = null;
                obj.lstDetail = null;
                obj.lstDetailBill = null;
                obj.lstDetailMultiple = null;                
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtpaymentMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PayId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno && x.TraccCode != obj.master.TrtypeAccount && x.TrentryType == "CHILD").OrderBy(x => x.TrserialNo).ToList();
                obj.lstDetailBill = db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno).OrderBy(x => x.PayId).ToList();
                obj.lstDetailMultiple = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno && x.TraccCode != obj.master.TrtypeAccount && x.Trdebit == 0 && x.TrentryType == "CHILD").OrderBy(x => x.TrserialNo).ToList();
                //obj.lstLedger = null;

                var query = from mas in obj.lstLedger
                            join account in obj.lstAccount on mas.TraccCode equals account.AccCode into accounts
                            from account in accounts.DefaultIfEmpty()
                            select new { mas };

                foreach(var j in query)
                {
                    Txtledger le = new Txtledger();
                    le = j.mas;
                    obj.lstLedger.Add(le);
                }

                if (obj.master.Trtype == "MULTIPLE")
                {
                    var query1 = from mas in obj.lstDetailMultiple
                                 join account in obj.lstAccountMultiple on mas.TraccCode equals account.AccCode into accounts
                                 from account in accounts.DefaultIfEmpty()
                                 select new { mas };

                    foreach (var j in query1)
                    {
                        Txtledger le = new Txtledger();
                        le = j.mas;
                        obj.lstDetailMultiple.Add(le);
                    }
                }
                else
                {
                    obj.lstLedger = null;
                }

                //obj.lstLedger = query.ToList();

                //obj.lstLedger = query.ToList();
                //obj.lstLedger = db.Txtledger.Join<,Txscoadetail,,>
                //obj.lstLedger = query.ToList();

                //List<query1> lst = obj.GetAllHyperlink();
                //return lst.GroupBy(x => x.Attraction) // group links by attraction
                //          .Select(g => g.First()) // select first link from each group
                //          .ToList(); // convert result to list

                //for (int i = 0; i < obj.lstLedger.Count; i++)
                //{
                //    EFJV eFJV = new EFJV();
                //    if (obj.lstLedger[i].Trdebit != 0) { eFJV.amount = obj.lstLedger[i].Trdebit; eFJV.debitCredit = "001"; }
                //    if (obj.lstLedger[i].Trcredit != 0) { eFJV.amount = obj.lstLedger[i].Trcredit; eFJV.debitCredit = "002"; }
                //    obj.eFJV.Add(eFJV);
                //}
                //obj.TotalCredit = obj.lstLedger.Sum(x => x.Trcredit);
                //obj.TotalDebit = obj.lstLedger.Sum(x => x.Trdebit);

            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentDetail(TXTPaymentMasterView obj, string Save, string Update, string Delete)
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
                db.TxtpaymentMaster.Add(obj.master);
                db.SaveChanges();

                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.lstLedger[i].Tramount != 0)
                    {
                        k = k + 1;
                        tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.master.TrtypeAccount, "", obj.lstLedger[i].TramountConverted, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);

                        if (obj.master.Trtype != "MULTIPLE")
                        {
                            k = k + 1;
                            tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.master.TrtypeAccount, obj.lstLedger[i].TraccCode, "", 0, obj.lstLedger[i].TramountWithTax, obj.master.TrexchangeRate, obj.lstLedger[i].TramountWithTax, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                        }
                    }
                    if (obj.lstLedger[i].TrtaxAmount != 0 && obj.lstLedger[i].TrtxsId != -1)
                    {
                        obj.lstAllTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxId == obj.lstLedger[i].TrtxsId).ToList();
                        if (obj.lstAllTax != null)
                        {
                            obj.lstTaxAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.Coaid == obj.lstAllTax[0].Coaid).OrderBy(x => x.AccName).ToList();
                        }

                        if (obj.lstTaxAccount != null)
                        {
                            k = k + 1;
                            if (obj.lstLedger[i].TramountWithTax > 0)
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", obj.lstLedger[i].TrtaxAmount, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxAmount, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                            else
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", 0, obj.lstLedger[i].TrtaxAmount * -1, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxAmount * -1, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                        }
                    }
                    if (obj.lstLedger[i].TrtaxExciseAmount != 0 && obj.lstLedger[i].TrtxsExciseId != -1)
                    {
                        obj.lstAllTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxId == obj.lstLedger[i].TrtxsExciseId).ToList();
                        if (obj.lstAllTax != null)
                        {
                            obj.lstTaxAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.Coaid == obj.lstAllTax[0].Coaid).OrderBy(x => x.AccName).ToList();
                        }

                        if (obj.lstTaxAccount != null)
                        {
                            k = k + 1;
                            if (obj.lstLedger[i].TramountWithTax > 0)
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", obj.lstLedger[i].TrtaxExciseAmount, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxExciseAmount, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                            else
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", 0, obj.lstLedger[i].TrtaxExciseAmount * -1, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxExciseAmount * -1, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                        }
                    }

                }

                if (obj.master.Trtype != "MULTIPLE")
                {
                    k = k + 1;
                    tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.master.TrtypeAccount, obj.master.TrtypeAccount, "", 0, obj.master.TrtotalAmount, obj.master.TrexchangeRate, obj.master.TrtotalAmount, obj.master.CurId, -1, 0, 0, "", null, obj.master.TrrefMain, obj.master.TrmainRemarks, "TOTAL", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "TOTAL", 0, 0, 0, obj.master.TrtotalAmount, obj.master.TrtotalAmount, -1, 0, 0);
                }
                else
                {
                    for (int i = 0; i < obj.lstDetailMultiple.Count; i++)
                    {
                        if (obj.lstDetailMultiple[i].TraccCode != "-1" && obj.lstDetailMultiple[i].Trcredit != 0)
                        {
                            k = k + 1;
                            tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstDetailMultiple[i].TraccCode, obj.lstDetailMultiple[i].TraccCode, "", 0, obj.lstDetailMultiple[i].TramountConverted, obj.master.TrexchangeRate, obj.lstDetailMultiple[i].TramountConverted, obj.master.CurId, -1, 0, 0, obj.lstDetailMultiple[i].TrchequeNo, obj.lstDetailMultiple[i].TrchequeDate, obj.master.TrrefMain, obj.master.TrmainRemarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.master.TrtotalAmount, obj.master.TrtotalAmount, -1, 0, 0);
                        }
                    }
                }

                obj.lstDetailBillCheck = db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno).OrderBy(x => x.PayId).ToList();
                for (int i = 0; i < obj.lstDetailBillCheck.Count; i++)
                {
                    double? TotalPaid = SupplierPaidAmount(obj.master.Trno, obj.lstDetailBillCheck[i].PblCode, obj.lstDetailBillCheck[i].PblSerialNo, obj.lstDetailBillCheck[i].InvCode, "NO");
                    TxtpurchaseDetail objUpdatePurchaseDetail = new TxtpurchaseDetail();
                    objUpdatePurchaseDetail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurPoref == obj.lstDetailBillCheck[i].PblCode && x.PurSerialNo == obj.lstDetailBillCheck[i].PblSerialNo).FirstOrDefault();
                    if (objUpdatePurchaseDetail != null)
                    {
                        objUpdatePurchaseDetail.PurPaidAmt = TotalPaid;
                        objUpdatePurchaseDetail.PurBalAmt = objUpdatePurchaseDetail.PurNetAmt - TotalPaid;
                        db.SaveChanges();
                    }
                }

                db.TxtpaymentBillDetail.RemoveRange(db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno));
                db.SaveChanges();

                for (int i = 0; i < obj.lstDetailBill.Count; i++)
                {
                    if (obj.lstDetailBill[i].PblCode != "" && obj.lstDetailBill[i].PblPaidAmount != 0)
                    {
                        k = k + 1;
                        tX.insertPaymentBill(user.Id, user.UserName, obj.master.Trno, obj.lstDetailBill[i].PblCode, obj.lstDetailBill[i].PblDate, k, obj.lstDetailBill[i].InvCode, obj.lstDetailBill[i].PblBillAmount, obj.lstDetailBill[i].PblOwingAmount, obj.lstDetailBill[i].PblPaidAmount, obj.lstDetailBill[i].PblBalanceAmount, obj.lstDetailBill[i].PblSubRemarks, "", obj.master.SupId);

                        double? TotalPaid = SupplierPaidAmount(obj.master.Trno, obj.lstDetailBillCheck[i].PblCode, obj.lstDetailBillCheck[i].PblSerialNo, obj.lstDetailBillCheck[i].InvCode, "YES");
                        TxtpurchaseDetail objUpdatePurchaseDetail = new TxtpurchaseDetail();
                        objUpdatePurchaseDetail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurPoref == obj.lstDetailBillCheck[i].PblCode && x.PurSerialNo == obj.lstDetailBillCheck[i].PblSerialNo).FirstOrDefault();
                        if (objUpdatePurchaseDetail != null)
                        {
                            objUpdatePurchaseDetail.PurPaidAmt = TotalPaid;
                            objUpdatePurchaseDetail.PurBalAmt = objUpdatePurchaseDetail.PurNetAmt - TotalPaid;
                            db.SaveChanges();
                        }
                    }
                }
            }

            if (Update != null)
            {
                TxtpaymentMaster objUpdate = new TxtpaymentMaster();
                objUpdate = db.TxtpaymentMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno).FirstOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.Trdate = obj.master.Trdate;
                    objUpdate.Trgldate = obj.master.Trgldate;
                    objUpdate.SitId = obj.master.SitId;
                    objUpdate.TrtotalAmount = obj.master.TrtotalAmount;
                    objUpdate.TrmainRemarks = obj.master.TrmainRemarks;
                    objUpdate.TrexchangeRate = obj.master.TrexchangeRate;
                    objUpdate.CurId = obj.master.CurId;
                    objUpdate.TrrefMain = obj.master.TrrefMain;
                    objUpdate.EditBy = user.UserName;
                    objUpdate.EditDate = DateTime.Now;
                    db.SaveChanges();
                }


                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                int k = 0;
                for (int i = 0; i < obj.lstLedger.Count; i++)
                {
                    if (obj.lstLedger[i].TraccCode != "-1" && obj.lstLedger[i].Tramount != 0)
                    {
                        k = k + 1;
                        tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstLedger[i].TraccCode, obj.master.TrtypeAccount, "", obj.lstLedger[i].TramountConverted, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TramountConverted, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);

                        if (obj.master.Trtype != "MULTIPLE")
                        {
                            k = k + 1;
                            tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.master.TrtypeAccount, obj.lstLedger[i].TraccCode, "", 0, obj.lstLedger[i].TramountWithTax, obj.master.TrexchangeRate, obj.lstLedger[i].TramountWithTax, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                        }
                    }
                    if (obj.lstLedger[i].TrtaxAmount != 0 && obj.lstLedger[i].TrtxsId != -1)
                    {
                        obj.lstAllTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxId == obj.lstLedger[i].TrtxsId).ToList();
                        if (obj.lstAllTax != null)
                        {
                            obj.lstTaxAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.Coaid == obj.lstAllTax[0].Coaid).OrderBy(x => x.AccName).ToList();
                        }

                        if (obj.lstTaxAccount != null)
                        {
                            k = k + 1;
                            if (obj.lstLedger[i].TramountWithTax > 0)
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", obj.lstLedger[i].TrtaxAmount, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxAmount, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                            else
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", 0, obj.lstLedger[i].TrtaxAmount * -1, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxAmount * -1, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                        }
                    }
                    if (obj.lstLedger[i].TrtaxExciseAmount != 0 && obj.lstLedger[i].TrtxsExciseId != -1)
                    {
                        obj.lstAllTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxActive == true && x.TaxType == "PURCHASE" && x.TaxId == obj.lstLedger[i].TrtxsExciseId).ToList();
                        if (obj.lstAllTax != null)
                        {
                            obj.lstTaxAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccActive == true && x.AccAccountType.Contains("TRANSACTION") && x.Coaid == obj.lstAllTax[0].Coaid).OrderBy(x => x.AccName).ToList();
                        }

                        if (obj.lstTaxAccount != null)
                        {
                            k = k + 1;
                            if (obj.lstLedger[i].TramountWithTax > 0)
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", obj.lstLedger[i].TrtaxExciseAmount, 0, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxExciseAmount, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                            else
                            {
                                tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstTaxAccount[0].AccCode, obj.lstTaxAccount[0].AccCode, "", 0, obj.lstLedger[i].TrtaxExciseAmount * -1, obj.master.TrexchangeRate, obj.lstLedger[i].TrtaxExciseAmount * -1, obj.master.CurId, obj.lstLedger[i].TrtxsId, obj.lstLedger[i].TrtaxPercent, obj.lstLedger[i].TrtaxAmount, obj.lstLedger[i].TrchequeNo, obj.lstLedger[i].TrchequeDate, obj.lstLedger[i].TrrefNo, obj.lstLedger[i].Trremarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.lstLedger[i].Tramount, obj.lstLedger[i].TramountWithTax, obj.lstLedger[i].TrtxsExciseId, obj.lstLedger[i].TrtaxExcisePercent, obj.lstLedger[i].TrtaxExciseAmount);
                            }
                        }
                    }

                }

                if (obj.master.Trtype != "MULTIPLE")
                {
                    k = k + 1;
                    tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.master.TrtypeAccount, obj.master.TrtypeAccount, "", 0, obj.master.TrtotalAmount, obj.master.TrexchangeRate, obj.master.TrtotalAmount, obj.master.CurId, -1, 0, 0, "", null, obj.master.TrrefMain, obj.master.TrmainRemarks, "TOTAL", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "TOTAL", 0, 0, 0, obj.master.TrtotalAmount, obj.master.TrtotalAmount, -1, 0, 0);
                }
                else
                {
                    for (int i = 0; i < obj.lstDetailMultiple.Count; i++)
                    {
                        if (obj.lstDetailMultiple[i].TraccCode != "-1" && obj.lstDetailMultiple[i].Trcredit != 0)
                        {
                            k = k + 1;
                            tX.insertLedgerPaymentDetail(user.Id, user.UserName, obj.master.Trno, obj.master.Trdate, obj.master.Trgldate, "PAYMENT", "UN-POST", k, obj.lstDetailMultiple[i].TraccCode, obj.lstDetailMultiple[i].TraccCode, "", 0, obj.lstDetailMultiple[i].TramountConverted, obj.master.TrexchangeRate, obj.lstDetailMultiple[i].TramountConverted, obj.master.CurId, -1, 0, 0, obj.lstDetailMultiple[i].TrchequeNo, obj.lstDetailMultiple[i].TrchequeDate, obj.master.TrrefMain, obj.master.TrmainRemarks, "CHILD", user.UserName, DateTime.Now, "Payment Detail", obj.master.SitId, "CHILD", 0, 0, 0, obj.master.TrtotalAmount, obj.master.TrtotalAmount, -1, 0, 0);
                        }
                    }
                }

                obj.lstDetailBillCheck = db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno).OrderBy(x => x.PayId).ToList();
                for (int i = 0; i < obj.lstDetailBillCheck.Count; i++)
                {
                    double? TotalPaid = SupplierPaidAmount(obj.master.Trno, obj.lstDetailBillCheck[i].PblCode, obj.lstDetailBillCheck[i].PblSerialNo, obj.lstDetailBillCheck[i].InvCode, "NO");
                    TxtpurchaseDetail objUpdatePurchaseDetail = new TxtpurchaseDetail();
                    objUpdatePurchaseDetail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurPoref == obj.lstDetailBillCheck[i].PblCode && x.PurSerialNo == obj.lstDetailBillCheck[i].PblSerialNo).FirstOrDefault();
                    if (objUpdatePurchaseDetail != null)
                    {
                        objUpdatePurchaseDetail.PurPaidAmt = TotalPaid;
                        objUpdatePurchaseDetail.PurBalAmt = objUpdatePurchaseDetail.PurNetAmt - TotalPaid;
                        db.SaveChanges();
                    }
                }

                db.TxtpaymentBillDetail.RemoveRange(db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno));
                db.SaveChanges();

                for (int i = 0; i < obj.lstDetailBill.Count; i++)
                {
                    if (obj.lstDetailBill[i].PblCode != "" && obj.lstDetailBill[i].PblPaidAmount != 0)
                    {
                        k = k + 1;
                        tX.insertPaymentBill(user.Id, user.UserName, obj.master.Trno, obj.lstDetailBill[i].PblCode, obj.lstDetailBill[i].PblDate, k, obj.lstDetailBill[i].InvCode, obj.lstDetailBill[i].PblBillAmount, obj.lstDetailBill[i].PblOwingAmount, obj.lstDetailBill[i].PblPaidAmount, obj.lstDetailBill[i].PblBalanceAmount, obj.lstDetailBill[i].PblSubRemarks, "", obj.master.SupId);

                        double? TotalPaid = SupplierPaidAmount(obj.master.Trno, obj.lstDetailBillCheck[i].PblCode, obj.lstDetailBillCheck[i].PblSerialNo, obj.lstDetailBillCheck[i].InvCode, "YES");
                        TxtpurchaseDetail objUpdatePurchaseDetail = new TxtpurchaseDetail();
                        objUpdatePurchaseDetail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurPoref == obj.lstDetailBillCheck[i].PblCode && x.PurSerialNo == obj.lstDetailBillCheck[i].PblSerialNo).FirstOrDefault();
                        if (objUpdatePurchaseDetail != null)
                        {
                            objUpdatePurchaseDetail.PurPaidAmt = TotalPaid;
                            objUpdatePurchaseDetail.PurBalAmt = objUpdatePurchaseDetail.PurNetAmt - TotalPaid;
                            db.SaveChanges();
                        }
                    }
                }

            }
            if (Delete != null)
            {
                db.Txtledger.RemoveRange(db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                db.TxtpaymentMaster.RemoveRange(db.TxtpaymentMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.Trno));
                db.SaveChanges();

                obj.lstDetailBillCheck = db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno).OrderBy(x => x.PayId).ToList();
                for (int i = 0; i < obj.lstDetailBillCheck.Count; i++)
                {
                    double? TotalPaid = SupplierPaidAmount(obj.master.Trno, obj.lstDetailBillCheck[i].PblCode, obj.lstDetailBillCheck[i].PblSerialNo, obj.lstDetailBillCheck[i].InvCode, "NO");
                    TxtpurchaseDetail objUpdatePurchaseDetail = new TxtpurchaseDetail();
                    objUpdatePurchaseDetail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurPoref == obj.lstDetailBillCheck[i].PblCode && x.PurSerialNo == obj.lstDetailBillCheck[i].PblSerialNo).FirstOrDefault();
                    if (objUpdatePurchaseDetail != null)
                    {
                        objUpdatePurchaseDetail.PurPaidAmt = TotalPaid;
                        objUpdatePurchaseDetail.PurBalAmt = objUpdatePurchaseDetail.PurNetAmt - TotalPaid;
                        db.SaveChanges();
                    }
                }

                db.TxtpaymentBillDetail.RemoveRange(db.TxtpaymentBillDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PblTrcode == obj.master.Trno));
                db.SaveChanges();
            }
            return RedirectToAction("showPayment");
        }

        #endregion


        #region Functions

        public List<Txscoadetail> PaymentCredit(string BankCash)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            List<Txscoadetail> lstCredit = null;
            lstCredit = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature == BankCash).ToList();
            //if (BankCash != "MULTIPLE")
            //{
            //    lstCredit = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature == BankCash).ToList();
            //}
            //else
            //{
            //    lstCredit = null;
            //}
            return lstCredit;
        }
        public List<Txscoadetail> ReceiptDebit(string BankCash)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            List<Txscoadetail> lstDebit = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature == BankCash).ToList();
            return lstDebit;
        }

        public List<Txscoadetail> CustomerSupplier(string CSName)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            List<Txscoadetail> lstCustSupp = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature == CSName).ToList();
            return lstCustSupp;
        }

        public TXTPaymentMasterView supplierChange(string Trno , int ExchangeRate, string supplier)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            TXTPaymentMasterView query = new TXTPaymentMasterView();
            List<TxtpurchaseDetail> purchase = new List<TxtpurchaseDetail>();
            List<TxtpaymentBillDetail> purchasePaid = new List<TxtpaymentBillDetail>();
            int supId = accCodeToSupplier(supplier);
            //Txtledger Ledger = new Txtledger();

            purchasePaid = db.TxtpaymentBillDetail.Where(x => x.Id == id && x.UserName == userName && x.SupId == supId && x.PblTrcode == Trno).ToList();
            if (purchasePaid != null)
            {
                for (int i = 0; i < purchasePaid.Count; i++)
                {
                    double? TotalPaid = SupplierPaidAmount1(Trno, purchasePaid[i].PblCode, purchasePaid[i].PblSerialNo, "NO");
                    if (TotalPaid != null)
                    {
                        TxtpaymentBillDetail payment = new TxtpaymentBillDetail();
                        payment.PblDate = purchasePaid[i].PblDate;
                        payment.PblCode = purchasePaid[i].PblCode;
                        payment.PblBillAmount = purchasePaid[i].PblBillAmount;
                        payment.PblOwingAmount = purchasePaid[i].PblBillAmount - Convert.ToDouble(TotalPaid);
                        payment.PblPaidAmount = purchasePaid[i].PblPaidAmount;
                        payment.PblBalanceAmount = purchasePaid[i].PblBillAmount - purchasePaid[i].PblPaidAmount - Convert.ToDouble(TotalPaid);
                        payment.PblSubRemarks = purchasePaid[i].PblSubRemarks;
                        query.lstDetailBill.Add(payment);
                    }
                }
            }

            purchase = db.TxtpurchaseDetail.Where(x => x.Id == id && x.UserName == userName && x.SupId == supId && x.PurPayTerm == "CREDIT" && x.PurPaidAmt == 0).ToList();
            if(purchase != null)
            {
                for (int i = 0; i < purchase.Count; i++)
                {
                    double? TotalPaid = SupplierPaidAmount1(Trno, purchase[i].PurPoref, purchase[i].PurSerialNo, "NO");
                    //string TotalPaid = db.Txtledger.Where(x => x.Id == id && x.UserName == userName && x.Trno != Trno && x.TrrefNo == purchase[i].PurPoref && x.TrentryTypeDoc == "CHILD").Sum(x => x.Trdebit - x.Trcredit).ToString();
                    if (TotalPaid != null)
                    {
                        TxtpaymentBillDetail payment = new TxtpaymentBillDetail();
                        payment.PblDate = purchase[i].PurDate;
                        payment.PblCode = purchase[i].PurPoref;
                        payment.PblBillAmount = purchase[i].PurNetAmt;
                        payment.PblOwingAmount = purchase[i].PurNetAmt - Convert.ToDouble(TotalPaid);
                        payment.PblPaidAmount = 0;
                        payment.PblBalanceAmount = purchase[i].PurNetAmt - Convert.ToDouble(TotalPaid);
                        payment.PblSubRemarks = purchase[i].PurRemarks;
                        query.lstDetailBill.Add(payment);
                    }
                }
            }
            return query;
        }
        public TXTReceiptMasterView customerChange(string Trno, int ExchangeRate, string customer)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            TXTReceiptMasterView query = new TXTReceiptMasterView();
            List<TxtsaleMaster> sale = new List<TxtsaleMaster>();
            int cusId = accCodeToCustomer(customer);
            Txtledger Ledger = new Txtledger();
            sale = db.TxtsaleMaster.Where(x => x.Id == id && x.UserName == userName && x.CusId == cusId && x.SalPayTerms == "DEBIT").ToList();
            if (sale != null)
            {
                for (int i = 0; i < sale.Count; i++)
                {
                    string TotalPaid = db.Txtledger.Where(x => x.Id == id && x.UserName == userName && x.Trno != Trno && x.TrrefNo == sale[i].SalSoRef && x.TrentryTypeDoc == "CHILD").Sum(x => x.Trdebit - x.Trcredit).ToString();
                    if (TotalPaid != null)
                    {
                        TxtreceiptDetail receipt = new TxtreceiptDetail();
                        receipt.RecInvDate = sale[i].SalDate;
                        receipt.RecInvNo = sale[i].SalSoRef;
                        receipt.RecOriginalAmt = Convert.ToDouble(db.TxtsaleDetail.Where(x => x.Id == id && x.UserName == userName && x.SalId == sale[i].SalId).Sum(x => x.SalGrossAmt).ToString());
                        receipt.RecAmtOwing = receipt.RecOriginalAmt - Convert.ToDouble(TotalPaid);
                        receipt.RecDiscAmt = Convert.ToDouble(db.TxtsaleDetail.Where(x => x.Id == id && x.UserName == userName && x.SalId == sale[i].SalId).Sum(x => x.SalDiscAmt).ToString());
                        receipt.RecReceiptAmt = receipt.RecOriginalAmt - Convert.ToDouble(TotalPaid);
                        receipt.RecExcAmt = (receipt.RecOriginalAmt - Convert.ToDouble(TotalPaid)) * ExchangeRate;
                        query.lstDetailOther.Add(receipt);
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
        public int accCodeToCustomer(string CusCode)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            var CoaId = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccCode == CusCode).FirstOrDefault().Coaid;
            return db.TxscustomerDetail.Where(x => x.Id == id && x.UserName == userName && x.CoaId == CoaId).FirstOrDefault().CusId;
        }
        public double? SupplierPaidAmount(string PblTrcode, string PblCode, double? PblSerialNo, string InvCode, string All)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            if (All == "YES")
            {
                return db.TxtpaymentBillDetail.Where(x => x.Id == id && x.UserName == userName && x.PblCode == PblCode && x.PblSerialNo == PblSerialNo && x.InvCode == InvCode).Sum(x => x.PblPaidAmount);
            }
            else
            {
                return db.TxtpaymentBillDetail.Where(x => x.Id == id && x.UserName == userName && x.PblTrcode != PblTrcode && x.PblCode == PblCode && x.PblSerialNo == PblSerialNo && x.InvCode == InvCode).Sum(x => x.PblPaidAmount);
            }
        }

        public double? SupplierPaidAmount1(string PblTrcode, string PblCode, double? PblSerialNo, string All)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            if (All == "YES")
            {
                return db.TxtpaymentBillDetail.Where(x => x.Id == id && x.UserName == userName && x.PblCode == PblCode && x.PblSerialNo == PblSerialNo).Sum(x => x.PblPaidAmount);
            }
            else
            {
                return db.TxtpaymentBillDetail.Where(x => x.Id == id && x.UserName == userName && x.PblTrcode != PblTrcode && x.PblCode == PblCode && x.PblSerialNo == PblSerialNo).Sum(x => x.PblPaidAmount);
            }
        }
    }
}
