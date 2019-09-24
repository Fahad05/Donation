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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Taxtation.App_Code;
using Taxtation.Models;
using Taxtation.Models.ManageViewModels;
using Taxtation.Services;
using Taxtation.ViewModel;

namespace Taxtation.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TransactionController : Controller
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
        public TransactionController(
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

        #region Purchase

        [HttpGet]
        public async Task<IActionResult> showPurchase()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTPurchaseDetailView obj = new TXTPurchaseDetailView();
            obj.lstMaster = db.TxtpurchaseMaster.Where(x => x.UserName == user.UserName).ToList();
            obj.detail.detail = db.TxtpurchaseDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstBank = db.TxsbankDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == user.UserName).ToList();


            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Purchase(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTPurchaseDetailView obj = new TXTPurchaseDetailView();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.PurPoref = tX.PurchaseOrder(user.UserName);
                obj.lstBank = db.TxsbankDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
                obj.lstTax = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "PURCHASE" && x.TaxActive == true).ToList();
                obj.lstExcise = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "SALE" && x.TaxActive == true).ToList();
                obj.detail.detail = null;
                obj.detail.pdef = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
            }
            return View(obj);
        }


        [HttpPost]
        public async Task<IActionResult> Purchase(TXTPurchaseDetailView obj, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(Save != null)
            {
                int count = 0;
                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    var j = Convert.ToInt32(Request.Form["detail_detail_" + i + "__ItmId"]);
                    if (obj.detail.detail[i].ItmId != Convert.ToInt32("-1") && obj.detail.detail[i].PurQty > 0 && obj.detail.detail[i].PurRate>0)
                    {
                        count++;
                    }
                }
                if(count > 0)
                {
                    obj.master.Id = user.Id;
                    obj.master.UserName = user.UserName;
                    obj.master.EnterBy = user.UserName;
                    obj.master.EnterDate = System.DateTime.Now;
                    db.TxtpurchaseMaster.Add(obj.master);
                    db.SaveChanges();

                    for (int i = 0; i < obj.detail.detail.Count; i++)
                    {
                        if (obj.detail.detail[i].ItmId != -1 && obj.detail.detail[i].PurQty > 0 && obj.detail.detail[i].PurRate > 0)
                        {
                            obj.detail.detail[i].Id = user.Id;
                            obj.detail.detail[i].UserName = user.UserName;
                            obj.detail.detail[i].PurSerialNo = i;
                            obj.detail.detail[i].PurId = obj.master.PurId;
                            db.TxtpurchaseDetail.Add(obj.detail.detail[i]);
                            db.SaveChanges();
                        }
                    }

                }
            }
            if(Update != null)
            {

            }
            return RedirectToAction("showPurchase");
        }


        #endregion


        #region Salae

        [HttpGet]
        public async Task<IActionResult> showSale()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTSaleDetailView sale = new TXTSaleDetailView();
            sale.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstCustomer=db.TxscustomerDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstExcise=db.TxstaxDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstItem=db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstSite=db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstStore=db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstTax=db.TxstaxDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstMaster=db.TxtsaleMaster.Where(x => x.UserName == user.UserName).ToList();
            sale.lstDetails = db.TxtsaleDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstBank = db.TxsbankDetail.Where(x => x.UserName == user.UserName).ToList();
            return View(sale);
        }

        [HttpGet]
        public async Task<IActionResult> Sale(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTSaleDetailView obj = new TXTSaleDetailView();
            obj.lstBank = db.TxsbankDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstCustomer = db.TxscustomerDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstExcise = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "SALE" && x.TaxActive == true).ToList();
            obj.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName ).ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "PURCHASE" && x.TaxActive == true).ToList();
            if(id==null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.SalSoRef = tX.SaleOrder(user.UserName);
                obj.Detail.saleDetail = null;
                obj.Detail.sale = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtsaleMaster.Where(x => x.SalId == Convert.ToInt32(id) && x.UserName == user.UserName).FirstOrDefault();
                obj.Detail.saleDetail = db.TxtsaleDetail.Where(x => x.UserName == user.UserName && x.SalId == Convert.ToInt32(id)).OrderBy(x => x.SalSerialNo).ToList();
                for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                {
                    
                }
            }
            
            return View(obj);
        }

        [HttpPost]
        public IActionResult Sale(TXTSaleDetailView obj)
        {
            

            return View();
        }



        #endregion


        #region Function

        public async Task<TxsitemDetail> changeItem(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            TxsitemDetail obj = new TxsitemDetail();
            Txsuomdetail uom = new Txsuomdetail();
            obj = db.TxsitemDetail.Where(x => x.UserName == user.UserName && x.ItmId == Convert.ToInt32(id)).OrderByDescending(x=>x.ItmId).FirstOrDefault();
            uom = db.Txsuomdetail.Where(x => x.Uomid == Convert.ToInt32(obj.ItmUom)).FirstOrDefault();
            obj.ItmUom = uom.Uomname;
            return obj;
        }

        public async Task<TxstaxDetail> changeTax(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            TxstaxDetail obj = new TxstaxDetail();
            obj = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxId == Convert.ToInt32(id)).FirstOrDefault();
            return obj;
        }


        #endregion
    }
}
