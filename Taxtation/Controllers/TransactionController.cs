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
            if (Save != null)
            {
                int count = 0;
                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    var j = Convert.ToInt32(Request.Form["detail_detail_" + i + "__ItmId"]);
                    if (obj.detail.detail[i].ItmId != Convert.ToInt32("-1") && obj.detail.detail[i].PurQty > 0 && obj.detail.detail[i].PurRate > 0)
                    {
                        count++;
                    }
                }
                if (count > 0)
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
            if (Update != null)
            {
                int count = 0;
                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    var j = Convert.ToInt32(Request.Form["detail_detail_" + i + "__ItmId"]);
                    if (obj.detail.detail[i].ItmId != Convert.ToInt32("-1") && obj.detail.detail[i].PurQty > 0 && obj.detail.detail[i].PurRate > 0)
                    {
                        count++;
                    }
                }
                if(count > 0)
                {
                    TxtpurchaseMaster master = new TxtpurchaseMaster();
                    master = db.TxtpurchaseMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == obj.master.PurId).FirstOrDefault();
                    master.PurDcref = obj.master.PurDcref;
                    master.PurDate = obj.master.PurDate;
                    master.PurSupDate = obj.master.PurSupDate;
                    master.PurType = obj.master.PurType;
                    master.SupId = obj.master.SupId;
                    master.PurPayTerm = obj.master.PurPayTerm;
                    master.BnkId = obj.master.BnkId;
                    master.PurChqNo = obj.master.PurChqNo;
                    master.PurChqDate = obj.master.PurChqDate;
                    master.PurScope = obj.master.PurScope;
                    master.PurPrices = obj.master.PurPrices;
                    master.PurRemarks = obj.master.PurRemarks;
                    master.PurActive = obj.master.PurActive;
                    master.PurPomapCode = obj.master.PurPomapCode;
                    master.CurId = obj.master.CurId;
                    master.StrId = obj.master.StrId;
                    master.PurExRate = obj.master.PurExRate;
                    master.PurItmType = obj.master.PurItmType;
                    master.SitId = obj.master.SitId;
                    master.EditBy = user.UserName;
                    master.EditDate = System.DateTime.Now;
                    db.SaveChanges();

                }
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
            sale.lstCustomer = db.TxscustomerDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstExcise = db.TxstaxDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstTax = db.TxstaxDetail.Where(x => x.UserName == user.UserName).ToList();
            sale.lstMaster = db.TxtsaleMaster.Where(x => x.UserName == user.UserName).ToList();
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
            obj.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "PURCHASE" && x.TaxActive == true).ToList();
            if (id == null)
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
        public async Task<IActionResult> Sale(TXTSaleDetailView obj, string Save, string Update)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(Save!=null)
            {
                int count = 0;
                for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                {
                    if(obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
                    {
                        count++;
                    }
                }
                if(count>0)
                {
                    obj.master.Id = user.Id;
                    obj.master.UserName = user.UserName;
                    obj.master.EnterBy = user.UserName;
                    obj.master.EnterDate = System.DateTime.Now;

                    db.TxtsaleMaster.Add(obj.master);
                    db.SaveChanges();

                    for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                    {
                        obj.Detail.saleDetail[i].Id = user.Id;
                        obj.Detail.saleDetail[i].UserName = user.UserName;
                        obj.Detail.saleDetail[i].SalId = obj.master.SalId;

                        db.TxtsaleDetail.Add(obj.Detail.saleDetail[i]);
                        db.SaveChanges();
                    }
                }
            }
            if(Update!=null)
            {
                int count = 0;
                for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                {
                    if (obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
                    {
                        count++;
                    }
                }
                if(count > 0)
                {
                    TxtsaleMaster master = new TxtsaleMaster();
                    master = db.TxtsaleMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SalId == obj.master.SalId).FirstOrDefault();
                    master.SalLporef = obj.master.SalLporef;
                    master.SalDate = obj.master.SalDate;
                    master.SalSupplyDate = obj.master.SalSupplyDate;
                    master.SalType = obj.master.SalType;
                    master.CusId = obj.master.CusId;
                    master.SalPayTerms = obj.master.SalPayTerms;
                    master.BnkId = obj.master.BnkId;
                    master.SalChqNo = obj.master.SalChqNo;
                    master.SalChqDate = obj.master.SalChqDate;
                    master.SalScope = obj.master.SalScope;
                    master.SalPrices = obj.master.SalPrices;
                    master.SalRemarks = obj.master.SalRemarks;
                    master.SalActive = obj.master.SalActive;
                    master.SalSalesMapCo = obj.master.SalSalesMapCo;
                    master.CurId = obj.master.CurId;
                    master.StrId = obj.master.StrId;
                    master.SalExRate = obj.master.SalExRate;
                    master.SalItemType = obj.master.SalItemType;
                    master.SitId = obj.master.SitId;
                    master.EditBy = user.UserName;
                    master.EditDate = System.DateTime.Now;
                    db.SaveChanges();

                    List<TxtsaleDetail> lstSale = new List<TxtsaleDetail>();
                    lstSale = db.TxtsaleDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SalId == obj.master.SalId).OrderBy(x => x.SalSerialNo).ToList();
                    for (int i = 0; i < lstSale.Count; i++)
                    {
                        db.TxtsaleDetail.Remove(lstSale[i]);
                        db.SaveChanges();
                    }

                    for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                    {
                        obj.Detail.saleDetail[i].Id = user.Id;
                        obj.Detail.saleDetail[i].UserName = user.UserName;
                        obj.Detail.saleDetail[i].SalId = obj.master.SalId;
                        db.TxtsaleDetail.Add(obj.Detail.saleDetail[i]);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("showSale");
        }



        #endregion



        #region Journal Detail

        //[HttpGet]
        //public async Task<IActionResult> JournalDetail(string id)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (User == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    if (id == null)
        //    {

        //        data.Tradjusting = (data.Tradjusting == null) ? true : false;
        //        return View(data);

        //    }
        //    else
        //    {


        //        data.Tradjusting = (data.Tradjusting == true) ? true : false;
        //        double? exchangeRate = data.TrexchangeRate;
        //        double? Debit = (from n in data.lstLedger select n.Trdebit).Sum();
        //        double? Credit = (from n in data.lstLedger select n.Trcredit).Sum();
        //        data.totalDebit = Debit * exchangeRate;
        //        data.totalCredit = Credit * exchangeRate;
        //        for (int i = 0; i < data.lstLedger.Count; i++)
        //        {
        //            if (data.lstLedger[i].Trdebit == 0)
        //            {
        //                data.lstLedger[i].Trdebit = 001;
        //                data.lstLedger[i].TrinvAmount = data.lstLedger[i].Trcredit;
        //            }
        //            if (data.lstLedger[i].Trcredit == 0)
        //            {
        //                data.lstLedger[i].TrinvAmount = data.lstLedger[i].Trdebit;
        //                data.lstLedger[i].Trdebit = 002;
        //            }
        //        }
        //        return View(data);

        //    }
        //    return View();
        //}


        //[HttpPost]
        //public IActionResult JournalDetail(GLTJournalMaster obj, string Save, string Find, string Update, string CB_Adjusting)
        //{
        //    ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        //    ViewData["UserCode"] = HttpContext.Session.GetString("UserCode");
        //    if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("UserCode") == null) { return RedirectToAction("Login", "Account"); }
        //    string serverUrl = _iconfiguration["ServerUrl"];
        //    obj.Tradjusting = (CB_Adjusting == "true") ? true : false;
        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(serverUrl);
        //    if (Save != null)
        //    {
        //        var myContent = JsonConvert.SerializeObject(obj);
        //        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //        var byteContent = new ByteArrayContent(buffer);
        //        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //        var response = client.PostAsync("postJournalDetail", byteContent).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("showJournalDetail", "JournalLedger");
        //        }
        //    }
        //    if (Update != null)
        //    {
        //        var myContent = JsonConvert.SerializeObject(obj);
        //        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //        var byteContent = new ByteArrayContent(buffer);
        //        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //        var response = client.PutAsync("updateJournalDetail", byteContent).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("showJournalDetail", "JournalLedger");
        //        }
        //    }
        //    return RedirectToAction("showJournalDetail", "JournalLedger");
        //}


        //[HttpGet]
        //public async Task<IActionResult> showJournalDetail()
        //{
        //    ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        //    ViewData["UserCode"] = HttpContext.Session.GetString("UserCode");
        //    if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("UserCode") == null) { return RedirectToAction("Login", "Account"); }
        //    string serverUrl = _iconfiguration["ServerUrl"];

        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(serverUrl);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    HttpResponseMessage response = await client.GetAsync("showJournalDetail");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        var data = JsonConvert.DeserializeObject<GLTJournalMasterView>(result);
        //        return View(data);
        //    }
        //    return View();
        //}

        #endregion

        #region Function

        public async Task<TxsitemDetail> changeItem(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            TxsitemDetail obj = new TxsitemDetail();
            Txsuomdetail uom = new Txsuomdetail();
            obj = db.TxsitemDetail.Where(x => x.UserName == user.UserName && x.ItmId == Convert.ToInt32(id)).OrderByDescending(x => x.ItmId).FirstOrDefault();
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
