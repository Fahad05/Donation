using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using Taxtation.Models;
using Taxtation.Services;
using Taxtation.ViewModel;

namespace Taxtation.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SetupController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        TAXTATIONContext db = new TAXTATIONContext();
        public SetupController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
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

        #region Currency


        [HttpGet]
        public async Task<IActionResult> showCurrency()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxscurrencyDetail> lstCurrency = new List<TxscurrencyDetail>();
            lstCurrency = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstCurrency);
        }

        [HttpGet]
        public async Task<IActionResult> Currency(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TxscurrencyDetail obj = new TxscurrencyDetail();
                obj.CurActive = (obj.CurActive == null) ? true : false;
                obj.CurIsLocal = (obj.CurIsLocal == null) ? false : false;

                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxscurrencyDetail obj = new TxscurrencyDetail();
                obj = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurId== Convert.ToInt32(id)).FirstOrDefault();
                if (obj != null) { }
                obj.CurActive = (obj.CurActive == true) ? true : false;
                obj.CurIsLocal = (obj.CurIsLocal == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Currency(TxscurrencyDetail obj, string Save, string Update, string CurActive, string CurIsLocal)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.Id = user.Id;
                obj.UserName = user.UserName;
                obj.CurActive = (CurActive == "true") ? true : false;
                obj.CurIsLocal = (CurIsLocal == "true") ? true : false;
                obj.EnterBy = user.UserName;
                obj.EnterDate = System.DateTime.Now;
                db.TxscurrencyDetail.Add(obj);
                db.SaveChanges();
            }
            if(Update!=null)
            {
                TxscurrencyDetail obj1 = new TxscurrencyDetail();
                obj1 = db.TxscurrencyDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CurId == obj.CurId).FirstOrDefault();
                if(obj1!=null)
                {
                    obj1.CurName = obj.CurName;
                    obj1.CurAbbr = obj.CurAbbr;
                    obj1.CurActive = (CurActive == "true") ? true : false;
                    obj1.CurIsLocal = (CurIsLocal == "true") ? true : false;
                    obj1.EditBy = Convert.ToString(user);
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("showCurrency");
        }

        public IActionResult PrintAllCurrency()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxscurrencyDetail> lstCurrency = new List<TxscurrencyDetail>();
            lstCurrency = db.TxscurrencyDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportCurrency", lstCurrency)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Store

        public async Task<IActionResult> showStore()
        {
            var user = await _userManager.GetUserAsync(User);
            if(User==null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsstoreDetail> lstStore = new List<TxsstoreDetail>();
            lstStore = db.TxsstoreDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstStore);
        }

        [HttpGet]
        public async Task<IActionResult> Store(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TxsstoreDetail obj = new TxsstoreDetail();
                obj.StrActive = (obj.StrActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxsstoreDetail obj = new TxsstoreDetail();
                obj = db.TxsstoreDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.StrId == Convert.ToInt32(id)).FirstOrDefault();
                obj.StrActive = (obj.StrActive == true) ? true : false;
                return PartialView(obj);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Store(TxsstoreDetail obj, string StrActive, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(Save!=null)
            {
                obj.Id = user.Id;
                obj.UserName = user.UserName;
                obj.StrActive = (obj.StrActive == true) ? true : false;
                obj.EnterBy = user.UserName;
                obj.EnterDate = System.DateTime.Now;
                db.TxsstoreDetail.Add(obj);
                db.SaveChanges();
            }
            if(Update!=null)
            {
                TxsstoreDetail obj1 = new TxsstoreDetail();
                obj1 = db.TxsstoreDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.StrId == obj.StrId).FirstOrDefault();
                if(obj1!=null)
                {
                    obj1.StrName = obj.StrName;
                    obj1.StrAbbr = obj.StrAbbr;
                    obj1.StrDesc = obj.StrDesc;
                    obj1.StrActive = (obj.StrActive == true) ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showStore");
        }

        public IActionResult PrintAllStore()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxsstoreDetail> lstStore = new List<TxsstoreDetail>();
            lstStore = db.TxsstoreDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportStore", lstStore)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Site

        public async Task<IActionResult> showSite()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxssiteDetail> lstSite = new List<TxssiteDetail>();
            lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstSite);
        }

        [HttpGet]
        public async Task<IActionResult> Site(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TxssiteDetail obj = new TxssiteDetail();
                obj.SitActive = (obj.SitActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxssiteDetail obj = new TxssiteDetail();
                obj = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitId == Convert.ToInt32(id)).FirstOrDefault();
                obj.SitActive = (obj.SitActive == true) ? true : false;
                return PartialView(obj);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Site(TxssiteDetail obj, string SitActive, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.Id = user.Id;
                obj.UserName = user.UserName;
                obj.SitActive = (obj.SitActive == true) ? true : false;
                obj.EnterBy = user.UserName;
                obj.EnterDate = System.DateTime.Now;
                db.TxssiteDetail.Add(obj);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxssiteDetail obj1 = new TxssiteDetail();
                obj1 = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitId == obj.SitId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.SitName = obj.SitName;
                    obj1.SitAbbr = obj.SitAbbr;
                    obj1.SitDesc = obj.SitDesc;
                    obj1.SitActive = (obj.SitActive == true) ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showSite");
        }
        public IActionResult PrintAllSites()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxssiteDetail> lstSite = new List<TxssiteDetail>();
            lstSite = db.TxssiteDetail.ToList();
        
            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportSites", lstSite)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches ="--footer-center \"  Page: [page]/[toPage]\"" +" --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };
        
        }

        #endregion

        #region Bank
        [HttpGet]
        public async Task<IActionResult> showBank()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsbankDetail> lstBank = new List<TxsbankDetail>();
            lstBank = db.TxsbankDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstBank);
        }
        
        [HttpGet]
        public async Task<IActionResult> Bank(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TxsbankDetail obj = new TxsbankDetail();
                obj.BnkActive = (obj.BnkActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxsbankDetail obj = new TxsbankDetail();
                obj = db.TxsbankDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.BnkId == Convert.ToInt32(id)).FirstOrDefault();
                obj.BnkActive = (obj.BnkActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bank(TxsbankDetail obj, string Save, string Update, string BnkActive)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(Save!=null)
            {
                obj.Id = user.Id;
                obj.UserName = user.UserName;
                obj.BnkActive = (obj.BnkActive == true) ? true : false;
                obj.EnterBy = user.UserName;
                obj.EnterDate = System.DateTime.Now;
                db.TxsbankDetail.Add(obj);
                db.SaveChanges();
            }
            if(Update!=null)
            {
                TxsbankDetail obj1 = new TxsbankDetail();
                obj1 = db.TxsbankDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.BnkId == obj.BnkId).FirstOrDefault();
                if(obj1!=null)
                {
                    obj1.BnkName = obj.BnkName;
                    obj1.BnkAbbr = obj.BnkAbbr;
                    obj1.BnkBranch = obj.BnkBranch;
                    obj1.BnkAccNo = obj.BnkAccNo;
                    obj1.BnkPhNo = obj.BnkPhNo;
                    obj1.BnkFax = obj.BnkFax;
                    obj1.BnkUrl = obj.BnkUrl;
                    obj1.BnkEmail = obj.BnkEmail;
                    obj1.BnkDesc = obj.BnkDesc;
                    obj1.BnkActive = (obj.BnkActive == true) ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showBank");
        }

        public IActionResult PrintAllBank()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxsbankDetail> lstBank = new List<TxsbankDetail>();
            lstBank = db.TxsbankDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportBank", lstBank)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Tax

        [HttpGet]
        public async Task<IActionResult> showTax()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxstaxDetail> lstTax = new List<TxstaxDetail>();
            lstTax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstTax);
        }

        [HttpGet]
        public async Task<IActionResult> Tax(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TxstaxDetail obj = new TxstaxDetail();
                obj.TaxActive = (obj.TaxActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxstaxDetail obj = new TxstaxDetail();
                obj = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == Convert.ToInt32(id)).FirstOrDefault();
                obj.TaxActive = (obj.TaxActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Tax(TxstaxDetail obj, string Save, string Update, string TaxActive)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.Id = user.Id;
                obj.UserName = user.UserName;
                obj.TaxActive = (obj.TaxActive == true) ? true : false;
                obj.EnterBy = user.UserName;
                obj.EnterDate = System.DateTime.Now;
                db.TxstaxDetail.Add(obj);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxstaxDetail obj1 = new TxstaxDetail();
                obj1 = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.TaxId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.TaxName = obj.TaxName;
                    obj1.TaxType = obj.TaxType;
                    obj1.TaxAbbr = obj.TaxAbbr;
                    obj1.TaxPercent = obj.TaxPercent;
                    obj1.TaxDesc = obj.TaxDesc;
                    obj1.TaxActive = (obj.TaxActive == true) ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showTax");
        }
        public IActionResult PrintAllTax()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxstaxDetail> lstTax = new List<TxstaxDetail>();
            lstTax = db.TxstaxDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportTax", lstTax)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Customer

        [HttpGet]
        public async Task<IActionResult> showCustomer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxscustomerDetail> lstCustomer = new List<TxscustomerDetail>();
            lstCustomer = db.TxscustomerDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstCustomer);
        }

        [HttpGet]
        public async Task<IActionResult> Customer(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TXSCustomerDetailView obj = new TXSCustomerDetailView();
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master.CusActive = (obj.master.CusActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TXSCustomerDetailView obj = new TXSCustomerDetailView();
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master = db.TxscustomerDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CusId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstCity = db.TxscityDetail.Where(x => x.CouCode == obj.master.CusCountry).ToList();
                obj.master.CusActive = (obj.master.CusActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Customer(TXSCustomerDetailView obj, string Save, string Update, string CusActive)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.master.Id = user.Id;
                obj.master.UserName = user.UserName;
                obj.master.CusActive = (CusActive == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxscustomerDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxscustomerDetail obj1 = new TxscustomerDetail();
                obj1 = db.TxscustomerDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.CusId == obj.master.CusId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.CusType = obj.master.CusType;
                    obj1.CusName = obj.master.CusName;
                    obj1.CusAbbr = obj.master.CusAbbr;
                    obj1.CusPerson = obj.master.CusPerson;
                    obj1.CusAddress = obj.master.CusAddress;
                    obj1.CusPhNo = obj.master.CusPhNo;
                    obj1.CusFaxNo = obj.master.CusFaxNo;
                    obj1.CusEmail = obj.master.CusEmail;
                    obj1.CusNtn = obj.master.CusNtn;
                    obj1.CusStrn = obj.master.CusStrn;
                    obj1.CusCity = obj.master.CusCity;
                    obj1.CusCountry = obj.master.CusCountry;
                    obj1.CusDesc = obj.master.CusDesc;
                    obj1.CusCrDays = obj.master.CusCrDays;
                    obj1.CusActive = (CusActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showCustomer");
        }
        public IActionResult PrintAllCustomer()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxscustomerDetail> lstCustomer = new List<TxscustomerDetail>();
            lstCustomer = db.TxscustomerDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportCustomer", lstCustomer)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }

        #endregion

        #region Supplier

        [HttpGet]
        public async Task<IActionResult> showSupplier()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxssupplierDetail> lstSupplier = new List<TxssupplierDetail>();
            lstSupplier = db.TxssupplierDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstSupplier);
        }

        [HttpGet]
        public async Task<IActionResult> Supplier(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TXSSupplierDetailView obj = new TXSSupplierDetailView();
                obj.master.SupActive = (obj.master.SupActive == null) ? true : false;
                obj.lstCountry = db.TxscountryDetail.ToList();
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TXSSupplierDetailView obj = new TXSSupplierDetailView();
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master = db.TxssupplierDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SupId == Convert.ToInt32(id)).FirstOrDefault();
                obj.master.SupActive = (obj.master.SupActive == true) ? true : false;
                obj.lstCity = db.TxscityDetail.Where(x => x.CouCode == obj.master.SupCountry).ToList();
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(TXSSupplierDetailView obj, string Save, string Update, string SupActive)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.master.Id = user.Id;
                obj.master.UserName = user.UserName;
                obj.master.SupActive = (SupActive == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxssupplierDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxssupplierDetail obj1 = new TxssupplierDetail();
                obj1 = db.TxssupplierDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SupId == obj.master.SupId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.SupType = obj.master.SupType;
                    obj1.SupName = obj.master.SupName;
                    obj1.SupAbbr = obj.master.SupAbbr;
                    obj1.SupPerson = obj.master.SupPerson;
                    obj1.SupAddress = obj.master.SupAddress;
                    obj1.SupPhNo = obj.master.SupPhNo;
                    obj1.SupFaxNo = obj.master.SupFaxNo;
                    obj1.SupEmail = obj.master.SupEmail;
                    obj1.SupNtn = obj.master.SupNtn;
                    obj1.SupStrn = obj.master.SupStrn;
                    obj1.SupCity = obj.master.SupCity;
                    obj1.SupCountry = obj.master.SupCountry;
                    obj1.SupDesc = obj.master.SupDesc;
                    obj1.SupCrDays = obj.master.SupCrDays;
                    obj1.SupActive = (SupActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showSupplier");
        }

        public IActionResult PrintAllSupplier()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxssupplierDetail> lstSupplier = new List<TxssupplierDetail>();
            lstSupplier = db.TxssupplierDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportSupplier", lstSupplier)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Item

        [HttpGet]
        public async Task<IActionResult> showItem()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsitemDetail> lstItem = new List<TxsitemDetail>();
            lstItem = db.TxsitemDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstItem);
        }

        [HttpGet]
        public async Task<IActionResult> Item(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                TXSItemDetailView obj = new TXSItemDetailView();
                obj.master.ItmActive = (obj.master.ItmActive == null) ? true : false;
                obj.master.ItmIsSale = (obj.master.ItmIsSale == null) ? false : false;
                obj.master.ItmIsPurchase = (obj.master.ItmIsPurchase == null) ? false : false;
                obj.master.ItmIsService = (obj.master.ItmIsService == null) ? false : false;
                obj.lstParent = db.TxsitemDetail.Where(x => x.ItmType != "ITEM").ToList();
                obj.lstStore = db.TxsstoreDetail.ToList();
                obj.lstUOM = db.Txsuomdetail.ToList();
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TXSItemDetailView obj = new TXSItemDetailView();
                obj.master = db.TxsitemDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ItmId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstParent = db.TxsitemDetail.Where(x => x.ItmId != Convert.ToInt32(id) && x.ItmType != "ITEM").ToList();
                obj.lstStore = db.TxsstoreDetail.ToList();
                obj.lstUOM = db.Txsuomdetail.ToList();
                obj.master.ItmActive = (obj.master.ItmActive == true) ? true : false;
                obj.master.ItmIsSale = (obj.master.ItmIsSale == true) ? true : false;
                obj.master.ItmIsPurchase = (obj.master.ItmIsPurchase == true) ? true : false;
                obj.master.ItmIsService = (obj.master.ItmIsService == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Item(TXSItemDetailView obj, string Save, string Update, string ItmActive, string ItmIsSale, string ItmIsPurchase, string ItmIsService)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.master.Id = user.Id;
                obj.master.UserName = user.UserName;
                obj.master.ItmActive = (ItmActive == "true") ? true : false;
                obj.master.ItmIsSale = (ItmIsSale == "true") ? true : false;
                obj.master.ItmIsPurchase = (ItmIsPurchase == "true") ? true : false;
                obj.master.ItmIsService = (ItmIsService == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxsitemDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxsitemDetail obj1 = new TxsitemDetail();
                obj1 = db.TxsitemDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ItmId == obj.master.ItmId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.ItmPid = obj.master.ItmPid;
                    obj1.ItmName = obj.master.ItmName;
                    obj1.ItmType = obj.master.ItmType;
                    obj1.ItmBcode = obj.master.ItmBcode;
                    obj1.ItmUom = obj.master.ItmUom;
                    obj1.ItmIsSale = (ItmIsSale == "true") ? true : false;
                    obj1.ItmIsPurchase = (ItmIsPurchase == "true") ? true : false;
                    obj1.ItmIsService = (ItmIsService == "true") ? true : false;
                    obj1.ItmMinLevel = obj.master.ItmMinLevel;
                    obj1.ItmMaxLevel = obj.master.ItmMaxLevel;
                    obj1.ItmReLevel = obj.master.ItmReLevel;
                    obj1.ItmCp = obj.master.ItmCp;
                    obj1.ItmSp = obj.master.ItmSp;
                    obj1.ItmOpQty = obj.master.ItmOpQty;
                    obj1.ItmOpStore = obj.master.ItmOpStore;
                    obj1.ItmActive = (ItmActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showItem");
        }

        public IActionResult PrintAllItem()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<TxsitemDetail> lstItem = new List<TxsitemDetail>();
            lstItem = db.TxsitemDetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportItem", lstItem)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        [HttpGet]
        public List<TxscityDetail> lstCity(string id)
        {
            List<TxscityDetail> lst = new List<TxscityDetail>();
            lst = db.TxscityDetail.Where(x => x.CouCode == id).ToList();
            return lst;
        }
    }
}