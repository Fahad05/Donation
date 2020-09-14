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
using Taxtation.App_Code;
using Taxtation.Models;
using Taxtation.Services;
using Taxtation.ViewModel;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace Taxtation.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SetupController : Controller
    {
        TX tX = new TX();
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

       
        #endregion

        #region Site

        public async Task<IActionResult> showSite()
        {
            var user = await _userManager.GetUserAsync(User);
            HttpContext.Session.SetString("UName", user.UserName);
            HttpContext.Session.SetString("UId", user.Id);
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
                obj.SitDefault = (obj.SitDefault == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxssiteDetail obj = new TxssiteDetail();
                obj = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitId == Convert.ToInt32(id)).FirstOrDefault();
                HttpContext.Session.SetInt32("SitId", Convert.ToInt32(id));
                obj.SitActive = (obj.SitActive == true) ? true : false;
                obj.SitDefault = (obj.SitDefault == true) ? true : false;
                HttpContext.Session.SetString("SitDefault", obj.SitDefault.ToString());
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
            TxssiteDetail objcheck = new TxssiteDetail();
            string id = HttpContext.Session.GetString("UId"), name = HttpContext.Session.GetString("UName");
            int sid = Convert.ToInt32(HttpContext.Session.GetInt32("SitId"));
            bool sdefault= Convert.ToBoolean(HttpContext.Session.GetString("SitDefault"));
            objcheck = db.TxssiteDetail.Where(x => x.Id == id && x.UserName == name && x.SitId != sid && x.SitDefault == true ).FirstOrDefault();
            //objcheck = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitId != obj.SitId && x.SitDefault == true && obj.SitDefault == true).FirstOrDefault();
            if (objcheck != null)
            {
                return RedirectToAction("showSite");
            }
            else
            {
                if (Save != null)
                {
                    obj.Id = user.Id;
                    obj.UserName = user.UserName;
                    obj.SitActive = (obj.SitActive == true) ? true : false;
                    obj.SitDefault = (obj.SitDefault == true) ? true : false;
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
                        obj1.SitDefault = (obj.SitDefault == true) ? true : false;
                        obj1.EditBy = user.UserName;
                        obj1.EditDate = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("showSite");
            }
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
                TXSTaxDetailView obj = new TXSTaxDetailView();
                obj.tax.Id = user.Id;
                obj.tax.UserName = user.UserName;
                obj.tax.TaxActive = (obj.tax.TaxActive == null) ? true : false;
                obj.tax.TxsDefault = (obj.tax.TxsDefault == null) ? true : false;
                obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "LIABILITY" && x.AccAccountSubNature == "TAX" && x.AccActive == true).ToList();
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TXSTaxDetailView obj = new TXSTaxDetailView();
                obj.tax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == Convert.ToInt32(id)).FirstOrDefault();
                obj.tax.TaxActive = (obj.tax.TaxActive == true) ? true : false;
                obj.tax.TxsDefault = (obj.tax.TxsDefault == true) ? true : false;
                if (obj.tax.TaxType == "SALE")
                {
                    obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "LIABILITY" && x.AccAccountSubNature == "TAX" && x.AccActive == true).ToList();
                }
                if (obj.tax.TaxType == "PURCHASE")
                {
                    obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "TAX" && x.AccActive == true).ToList();
                }
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Tax(TXSTaxDetailView obj, string Save, string Update, string TaxActive, string TxsDefault)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Save != null)
            {
                obj.tax.Id = user.Id;
                obj.tax.UserName = user.UserName;
                obj.tax.TaxActive = (TaxActive == "true") ? true : false;
                obj.tax.TxsDefault = (TaxActive == "true") ? true : false;
                obj.tax.EnterBy = user.UserName;
                obj.tax.EnterDate = System.DateTime.Now;
                db.TxstaxDetail.Add(obj.tax);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxstaxDetail obj1 = new TxstaxDetail();
                obj1 = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.tax.TaxId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.TaxName = obj.tax.TaxName;
                    obj1.TaxType = obj.tax.TaxType;
                    obj1.TaxAbbr = obj.tax.TaxAbbr;
                    obj1.TaxPercent = obj.tax.TaxPercent;
                    obj1.TaxDesc = obj.tax.TaxDesc;
                    obj1.Coaid = obj.tax.Coaid;
                    obj1.TaxActive = (TaxActive == "true") ? true : false;
                    obj1.TxsDefault = (TaxActive == "true") ? true : false;
                    obj1.TaxCategory = obj.tax.TaxCategory;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showTax");
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
            TXSCustomerDetailView obj = new TXSCustomerDetailView();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountNature == "LIABILITY").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master.CusActive = (obj.master.CusActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
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
                    obj1.CoaId = obj.master.CoaId;
                    obj1.CusActive = (CusActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showCustomer");
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
            TXSSupplierDetailView obj = new TXSSupplierDetailView();
            obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountNature == "ASSET").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                
                obj.master.SupActive = (obj.master.SupActive == null) ? true : false;
                obj.lstCountry = db.TxscountryDetail.ToList();
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
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
                    obj1.CoaId = obj.master.CoaId;
                    obj1.SupActive = (SupActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showSupplier");
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
                obj.lstAssetAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "NORMAL").ToList();
                obj.lstInventoryAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "NORMAL").ToList();
                obj.lstExpenseAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "EXPENSE").ToList();
                obj.lstRevenueAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "REVENUE").ToList();
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
                obj.lstAssetAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "NORMAL").ToList();
                obj.lstInventoryAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "NORMAL").ToList();
                obj.lstExpenseAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "EXPENSE").ToList();
                obj.lstRevenueAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "REVENUE").ToList();
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
                    obj1.ItmExpenseAccount = obj.master.ItmExpenseAccount;
                    obj1.ItmRevenueAccount = obj.master.ItmRevenueAccount;
                    obj1.ItmCogsaccount = obj.master.ItmCogsaccount;
                    obj1.ItmAssetAccount = obj.master.ItmAssetAccount;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showItem");
        }

      
        #endregion


        #region COA

        public async Task<IActionResult> showCOA()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<Txscoadetail> lstCOA = new List<Txscoadetail>();
            lstCOA = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstCOA);
        }

        [HttpGet]
        public async Task<IActionResult> COA(string id)
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
                ViewData["_HiddenCode"] = "0";
                ViewData["_ParentAccountCode"] = "0";
                TXSCOADetailView obj = new TXSCOADetailView();
                obj.coa.AccActive = (obj.coa.AccActive == null) ? true : false;
                obj.lstCoa = db.Txscoadetail.Where(x=>x.UserName==user.UserName).ToList();
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TXSCOADetailView obj = new TXSCOADetailView();
                obj.coa = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Coaid == Convert.ToInt32(id)).FirstOrDefault();
                obj.coa.AccActive = (obj.coa.AccActive == true) ? true : false;
                ViewData["_HiddenCode"] = obj.coa.AccCode;
                ViewData["_ParentAccountCode"] = obj.coa.AccParentAccount;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> COA(TXSCOADetailView obj, string AccActive, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            obj.coa.Id = user.Id;
            obj.coa.UserName = user.UserName;
            obj.coa.AccActive = (AccActive == "true") ? true : false;
            obj.coa.AccHierarchyCode = obj.coa.AccCode;
            obj.coa.AccGrpCode = obj.coa.AccAccountNature;
            obj.coa.AccTaxAccount = false;
            obj.coa.AccAllowBudgeting = false;
            obj.coa.AccAllowPosting = false;
            obj.coa.AccReconcile = false;
            obj.coa.AccThirdParty = false;
            if (Save!=null)
            {
                obj.coa.EnterBy = user.UserName;
                obj.coa.EnterDate = System.DateTime.Now;
                db.Txscoadetail.Add(obj.coa);
                db.SaveChanges();
            }
            if(Update != null)
            {
                Txscoadetail txscoadetail = new Txscoadetail();
                txscoadetail = db.Txscoadetail.Where(x => x.Id == obj.coa.Id && x.UserName == user.UserName && x.Coaid == obj.coa.Coaid).FirstOrDefault();
                txscoadetail.AccName = obj.coa.AccName;
                txscoadetail.AccAbbr = obj.coa.AccAbbr;
                txscoadetail.AccDesc = obj.coa.AccDesc;
                txscoadetail.AccHierarchyCode = obj.coa.AccCode;
                txscoadetail.AccGrpCode = obj.coa.AccAccountNature;

                txscoadetail.AccComCode = obj.coa.AccComCode;
                txscoadetail.AccBsuCode = obj.coa.AccBsuCode;
                txscoadetail.AccParentAccount = obj.coa.AccParentAccount;
                txscoadetail.AccAccountType = obj.coa.AccAccountType;
                txscoadetail.AccAccountNature = obj.coa.AccAccountNature;
                txscoadetail.AccAccountSubNature = obj.coa.AccAccountSubNature;
                txscoadetail.AccTaxAccount = obj.coa.AccTaxAccount;
                txscoadetail.AccAllowBudgeting = obj.coa.AccAllowBudgeting;
                txscoadetail.AccReconcile = obj.coa.AccReconcile;
                txscoadetail.AccThirdParty = obj.coa.AccThirdParty;
                txscoadetail.AccEffectPeriodStart = obj.coa.AccEffectPeriodStart;
                txscoadetail.AccEffectPeriodEnd = obj.coa.AccEffectPeriodEnd;
                txscoadetail.AccActive = obj.coa.AccActive;
                txscoadetail.EditOutSide = obj.coa.EditOutSide;
                txscoadetail.OpeningBalance = obj.coa.OpeningBalance;
                txscoadetail.ClosingBalance = obj.coa.ClosingBalance;
                txscoadetail.OpeningBalanceCr = obj.coa.OpeningBalanceCr;
                txscoadetail.ClosingBalanceCr = obj.coa.ClosingBalanceCr;
                txscoadetail.TransactionDr = obj.coa.TransactionDr;
                txscoadetail.TransactionCr = obj.coa.TransactionCr;
                txscoadetail.AccLevel = obj.coa.AccLevel;
                txscoadetail.TopeningBalance = obj.coa.TopeningBalance;
                txscoadetail.TclosingBalance = obj.coa.TclosingBalance;
                txscoadetail.TopeningBalanceCr = obj.coa.TopeningBalanceCr;
                txscoadetail.TclosingBalanceCr = obj.coa.TopeningBalanceCr;
                txscoadetail.TtransactionDr = obj.coa.TtransactionDr;
                txscoadetail.TtransactionCr = obj.coa.TtransactionCr;
                txscoadetail.Pl = obj.coa.Pl;
                txscoadetail.Bs = obj.coa.Bs;
                txscoadetail.Cf = obj.coa.Cf;
                txscoadetail.Ce = obj.coa.Ce;
                txscoadetail.Cf1 = obj.coa.Cf1;
                txscoadetail.Cf2 = obj.coa.Cf2;
                txscoadetail.Ce1 = obj.coa.Ce1;
                txscoadetail.Ce2 = obj.coa.Ce2;
                txscoadetail.AccLevelNo = obj.coa.AccLevelNo;
                txscoadetail.AccNameLevelWise = obj.coa.AccNameLevelWise;
                txscoadetail.AccAccountTypeShort = obj.coa.AccAccountTypeShort;
                txscoadetail.AccOpeningBalance = obj.coa.AccOpeningBalance;
                obj.coa.EditBy = user.UserName;
                obj.coa.EditDate = System.DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("showCOA");
        }



        #endregion


        //#region Project

        //[HttpGet]
        //public async Task<IActionResult> showProject()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (User == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }
        //    List<TxsprojectDetail> lstProj = new List<TxsprojectDetail>();
        //    lstProj = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
        //    return View(lstProj);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Project(string id)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (User == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }
        //    if (id == null)
        //    {
        //        ViewData["_Save"] = "True";
        //        ViewData["_Update"] = "False";
        //        TxsprojectDetail obj = new TxsprojectDetail();
        //        obj.ProActive = (obj.ProActive == null) ? true : false;
        //        return PartialView(obj);
        //    }
        //    else
        //    {
        //        ViewData["_Save"] = "False";
        //        ViewData["_Update"] = "True";
        //        TxsprojectDetail obj = new TxsprojectDetail();
        //        obj = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ProId == Convert.ToInt32(id)).FirstOrDefault();
        //        obj.ProActive = (obj.ProActive == true) ? true : false;
        //        return PartialView(obj);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Project(TxsprojectDetail obj, string ProActive, string Save, string Update)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (User == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }
        //    if (Save != null)
        //    {
        //        obj.Id = user.Id;
        //        obj.UserName = user.UserName;
        //        obj.ProActive = (ProActive == "true") ? true : false;
        //        obj.EnterBy = user.UserName;
        //        obj.EnterDate = System.DateTime.Now;
        //        db.TxsprojectDetail.Add(obj);
        //        db.SaveChanges();
        //    }
        //    if (Update != null)
        //    {
        //        TxsprojectDetail obj1 = new TxsprojectDetail();
        //        obj1 = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ProId == obj.ProId).FirstOrDefault();
        //        if (obj1 != null)
        //        {
        //            obj1.ProName = obj.ProName;
        //            obj1.ProAbbr = obj.ProAbbr;
        //            obj1.ProDesc = obj.ProDesc;
        //            obj1.ProActive = (ProActive == "true") ? true : false;
        //            obj1.EditBy = user.UserName;
        //            obj1.EditDate = System.DateTime.Now;
        //            db.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("showProject");
        //}

        //#endregion

        #region Project

        [HttpGet]
        public async Task<IActionResult> showProject()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsprojectDetail> lstProj = new List<TxsprojectDetail>();
            lstProj = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstProj);
        }

        [HttpGet]
        public async Task<IActionResult> Project(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXSProjectDetailView obj = new TXSProjectDetailView();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";

                obj.lstOrg = db.TxsorganizationDetail.ToList();
                obj.master.ProActive = (obj.master.ProActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ProId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstOrg = db.TxsorganizationDetail.Where(x => x.OrgId == obj.master.ProOrg).ToList();
                obj.master.ProActive = (obj.master.ProActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Project(TXSProjectDetailView obj, string ProActive, string Save, string Update)
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
                obj.master.ProActive = (ProActive == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxsprojectDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxsprojectDetail obj1 = new TxsprojectDetail();
                obj1 = db.TxsprojectDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ProId == obj.master.ProId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.ProName = obj.master.ProName;
                    obj1.ProAbbr = obj.master.ProAbbr;
                    obj1.ProDesc = obj.master.ProDesc;
                    obj1.ProOrg = obj.master.ProOrg;
                    obj1.ProBudget = obj.master.ProBudget;
                    obj1.ProStartDate = obj.master.ProStartDate;
                    obj1.ProEndDate = obj.master.ProEndDate;
                    obj1.ProActive = (ProActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showProject");
        }

        #endregion

        #region Donation Category

        public async Task<IActionResult> showDonationCategory()
        {
            var user = await _userManager.GetUserAsync(User);
            HttpContext.Session.SetString("UName", user.UserName);
            HttpContext.Session.SetString("UId", user.Id);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsdonationCategoryDetail> lstDonationCategory = new List<TxsdonationCategoryDetail>();
            lstDonationCategory = db.TxsdonationCategoryDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstDonationCategory);
        }

        [HttpGet]
        public async Task<IActionResult> DonationCategory(string id)
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
                TxsdonationCategoryDetail obj = new TxsdonationCategoryDetail();
                obj.DcaActive = (obj.DcaActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                TxsdonationCategoryDetail obj = new TxsdonationCategoryDetail();
                obj = db.TxsdonationCategoryDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DcaId == Convert.ToInt32(id)).FirstOrDefault();
                HttpContext.Session.SetInt32("DcaId", Convert.ToInt32(id));
                obj.DcaActive = (obj.DcaActive == true) ? true : false;
                return PartialView(obj);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DonationCategory(TxsdonationCategoryDetail obj, string DcaActive, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TxsdonationCategoryDetail objcheck = new TxsdonationCategoryDetail();
            string id = HttpContext.Session.GetString("UId"), name = HttpContext.Session.GetString("UName");
            int sid = Convert.ToInt32(HttpContext.Session.GetInt32("DcaId"));
            objcheck = db.TxsdonationCategoryDetail.Where(x => x.Id == id && x.UserName == name && x.DcaId != sid).FirstOrDefault();
            //objcheck = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SitId != obj.SitId && x.SitDefault == true && obj.SitDefault == true).FirstOrDefault();
            if (objcheck != null)
            {
                return RedirectToAction("showDonationCategory");
            }
            else
            {
                if (Save != null)
                {
                    obj.Id = user.Id;
                    obj.UserName = user.UserName;
                    obj.DcaActive = (obj.DcaActive == true) ? true : false;
                    obj.EnterBy = user.UserName;
                    obj.EnterDate = System.DateTime.Now;
                    db.TxsdonationCategoryDetail.Add(obj);
                    db.SaveChanges();
                }
                if (Update != null)
                {
                    TxsdonationCategoryDetail obj1 = new TxsdonationCategoryDetail();
                    obj1 = db.TxsdonationCategoryDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DcaId == obj.DcaId).FirstOrDefault();
                    if (obj1 != null)
                    {
                        obj1.DcaName = obj.DcaName;
                        obj1.DcaAbbr = obj.DcaAbbr;
                        obj1.DcaDesc = obj.DcaDesc;
                        obj1.DcaActive = (obj.DcaActive == true) ? true : false;
                        obj1.EditBy = user.UserName;
                        obj1.EditDate = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("showDonationCategory");
            }
        }


        #endregion

        #region Donor

        [HttpGet]
        public async Task<IActionResult> showDonor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsdonorDetail> lstDonor = new List<TxsdonorDetail>();
            lstDonor = db.TxsdonorDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();

            return View(lstDonor);
        }

        [HttpGet]
        public async Task<IActionResult> Donor(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXSDonorDetailView obj = new TXSDonorDetailView();
            // obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountNature == "LIABILITY").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";

                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master.DnrActive = (obj.master.DnrActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master = db.TxsdonorDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DnrId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstCity = db.TxscityDetail.Where(x => x.CouCode == obj.master.DnrCountry).ToList();
                obj.master.DnrActive = (obj.master.DnrActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Donor(TXSDonorDetailView obj, string Save, string Update, string DnrActive)
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
                obj.master.DnrActive = (DnrActive == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxsdonorDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxsdonorDetail obj1 = new TxsdonorDetail();
                obj1 = db.TxsdonorDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DnrId == obj.master.DnrId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.DnrName = obj.master.DnrName;
                    obj1.DnrAddress = obj.master.DnrAddress;
                    obj1.DnrPhNo = obj.master.DnrPhNo;
                    obj1.DnrFaxNo = obj.master.DnrFaxNo;
                    obj1.DnrEmail = obj.master.DnrEmail;
                    obj1.DnrNtn = obj.master.DnrNtn;
                    obj1.DnrStrn = obj.master.DnrStrn;
                    obj1.DnrCity = obj.master.DnrCity;
                    obj1.DnrCountry = obj.master.DnrCountry;
                    obj1.DnrDesc = obj.master.DnrDesc;
                    obj1.DnrActive = (DnrActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showDonor");
        }

        #endregion

        #region Organization

        [HttpGet]
        public async Task<IActionResult> showOrganization()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxsorganizationDetail> lstOrganization = new List<TxsorganizationDetail>();
            lstOrganization = db.TxsorganizationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();

            return View(lstOrganization);
        }

        [HttpGet]
        public async Task<IActionResult> Organization(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXSOrganizationDetailView obj = new TXSOrganizationDetailView();
            // obj.lstAccount = db.Txscoadetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.AccAccountNature == "LIABILITY").ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";

                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master.OrgActive = (obj.master.OrgActive == null) ? true : false;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.lstCountry = db.TxscountryDetail.ToList();
                obj.master = db.TxsorganizationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.OrgId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstCity = db.TxscityDetail.Where(x => x.CouCode == obj.master.OrgCountry).ToList();
                obj.master.OrgActive = (obj.master.OrgActive == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Organization(TXSOrganizationDetailView obj, string Save, string Update, string OrgActive)
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
                obj.master.OrgActive = (OrgActive == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxsorganizationDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxsorganizationDetail obj1 = new TxsorganizationDetail();
                obj1 = db.TxsorganizationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.OrgId == obj.master.OrgId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.OrgName = obj.master.OrgName;
                    obj1.OrgAddress = obj.master.OrgAddress;
                    obj1.OrgPhNo = obj.master.OrgPhNo;
                    obj1.OrgFaxNo = obj.master.OrgFaxNo;
                    obj1.OrgEmail = obj.master.OrgEmail;
                    obj1.OrgNtn = obj.master.OrgNtn;
                    obj1.OrgStrn = obj.master.OrgStrn;
                    obj1.OrgCity = obj.master.OrgCity;
                    obj1.OrgCountry = obj.master.OrgCountry;
                    obj1.OrgDesc = obj.master.OrgDesc;
                    obj1.OrgActive = (OrgActive == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showOrganization");
        }

        #endregion


        [HttpGet]
        public List<Txscoadetail> lstCOAAccount(string id, string userName, string type)
        {
            List<Txscoadetail> lst = new List<Txscoadetail>();
            if (type == "SALE")
            {
                lst = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "LIABILITY" && x.AccAccountSubNature == "TAX" && x.AccActive == true).ToList();
            }
            if (type == "PURCHASE")
            {
                lst = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountType == "TRANSACTION" && x.AccAccountNature == "ASSET" && x.AccAccountSubNature == "TAX" && x.AccActive == true).ToList();
            }
            return lst;
        }


        [HttpGet]
        public List<TxscityDetail> lstCity(string id)
        {
            List<TxscityDetail> lst = new List<TxscityDetail>();
            lst = db.TxscityDetail.Where(x => x.CouCode == id).ToList();
            return lst;
        }

        [HttpGet]
        public IActionResult getParentAccountDetail(string AccountNature, string AccCode)
        {
            List<Txscoadetail> obj = new List<Txscoadetail>();
            obj = db.Txscoadetail.Where(x => x.AccAccountNature == AccountNature && x.AccCode != AccCode && x.AccAccountType == "CONTROL").OrderBy(x => x.AccName).ToList();
            return Ok(obj);
        }

        [HttpGet]
        public IActionResult getParentAccountCodeDetail(string AccCode, string len)
        {
            string parentCode = "";
            parentCode = tX.getParentAccountCodeDetail(AccCode);
            //if (parentCode != null)
            //{
            if (len == "1") { if (parentCode != null) { parentCode = (Convert.ToInt32(Right(parentCode, 3)) + 1).ToString("D3"); } else { parentCode = "001"; } }
            if (len == "5") { if (parentCode != null) { parentCode = (Convert.ToInt32(Right(parentCode, 4)) + 1).ToString("D4"); } else { parentCode = "0001"; } }
            if (len == "10") { if (parentCode != null) { parentCode = (Convert.ToInt32(Right(parentCode, 5)) + 1).ToString("D5"); } else { parentCode = "00001"; } }
            if (Convert.ToInt32(len) >= 16) { if (parentCode != null) { parentCode = (Convert.ToInt32(Right(parentCode, 6)) + 1).ToString("D6"); } else { parentCode = "000001"; } }
            //}
            return Json(parentCode);
        }

        [HttpGet]
        public IActionResult findParentAccount(string table, string AccCode)
        {
            if (table == "Txscoadetail")
            {
                Txscoadetail obj = new Txscoadetail();
                obj = tX.findParentAccount(AccCode);
                return Ok(obj);
            }
            //if (table == "GltreceiptMaster")
            //{
            //    //TRTypeAccount
            //    GltreceiptMaster obj = new GltreceiptMaster();
            //    obj = setup.findParentAccountReceiptMaster(AccCode);
            //    return Ok(obj);
            //}
            //if (table == "GltpaymentMaster")
            //{
            //    //TRTypeAccount
            //    GltpaymentMaster obj = new GltpaymentMaster();
            //    obj = setup.findParentAccountPaymentMaster(AccCode);
            //    return Ok(obj);
            //}
            return Ok();
        }


        #region Methods

        public string Left(string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public string Right(string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }
        public JsonResult DefaultCheck()
        {
            TxssiteDetail objcheck1 = new TxssiteDetail();
            objcheck1 = null;
            string id= HttpContext.Session.GetString("UId"), name= HttpContext.Session.GetString("UName");
            int sid = Convert.ToInt32(HttpContext.Session.GetInt32("SitId"));
            bool sdefault = Convert.ToBoolean(HttpContext.Session.GetString("SitDefault"));
            objcheck1 = db.TxssiteDetail.Where(x => x.Id == id && x.UserName == name && x.SitId != sid && x.SitDefault == true).FirstOrDefault();
            return Json(objcheck1);
        }
        #endregion
    }
}
