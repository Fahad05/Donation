﻿using System;
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
    public class TransactionController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        static string UId;
        static string UName;
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
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            TXTPurchaseDetailView obj = new TXTPurchaseDetailView();
            UId = user.Id;
            UName = user.UserName;
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
            obj.master.Id = user.Id;
            obj.master.UserName = user.UserName;
            obj.lstBank = db.TxsbankDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            obj.lstTax = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "PURCHASE" && x.TaxActive == true).ToList();
            obj.lstExcise = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxType == "SALE" && x.TaxActive == true).ToList();

            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                obj.master.PurPoref = tX.PurchaseOrder(user.UserName);
                obj.lstAccount = db.Txscoadetail.Where(x => x.UserName == user.UserName && (x.AccAccountSubNature.Contains("CASH") || x.AccAccountSubNature.Contains("BANK ACCOUNT")) && x.AccActive == true).ToList();
                obj.detail.detail = null;
                obj.detail.pdef = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";

                obj.master = db.TxtpurchaseMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).FirstOrDefault();
                if (obj.master.PurPayTerm == "CREDIT") { obj.lstAccount = db.Txscoadetail.Where(x => x.UserName == user.UserName && (x.AccAccountSubNature.Contains("CASH") || x.AccAccountSubNature.Contains("BANK ACCOUNT")) && x.AccActive == true).ToList(); }
                else { obj.lstAccount = db.Txscoadetail.Where(x => x.UserName == user.UserName && x.AccAccountSubNature.Contains(obj.master.PurPayTerm) && x.AccActive == true).ToList(); }
                obj.detail.detail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).OrderBy(x => x.PurSerialNo).ToList();
                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    PDEF pDEF = new PDEF();
                    TXSItemUOMDetail item = new TXSItemUOMDetail();
                    int itmid = (int)obj.detail.detail[i].ItmId;
                    if (itmid != -1)
                    {
                        item = changeItems(itmid, user.Id, user.UserName);
                        pDEF.UOM = item.Txuom.Uomname;
                        pDEF.lastPrice = item.Txsitem.ItmSp;
                    }

                    pDEF.subAmount = obj.detail.detail[i].PurQty * obj.detail.detail[i].PurRate;
                    pDEF.AmtAfterExcise = pDEF.subAmount + obj.detail.detail[i].PurExAmt;
                    pDEF.AmtAfterDiscount = pDEF.subAmount + obj.detail.detail[i].PurExAmt - obj.detail.detail[i].PurDiscountAmt;
                    obj.detail.pdef.Add(pDEF);
                    obj.detail.detail[i].PurGrossAmt = obj.detail.detail[i].PurNetAmt * obj.master.PurExRate;
                }
            }
            return View(obj);
        }


        [HttpPost]
        public async Task<IActionResult> Purchase(TXTPurchaseDetailView obj, string Save, string Update)
        {
            if (ModelState.IsValid)
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
                    obj.master.EnterBy = user.UserName;
                    obj.master.EnterDate = System.DateTime.Now;
                    db.TxtpurchaseMaster.Add(obj.master);
                    db.SaveChanges();
                    string InventoryAccType = ""; int k = 0;
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

                            tX.insertInventoryStockDetail(user.Id, user.UserName, obj.detail.detail[i].ItmId, i, obj.master.PurPoref, obj.master.PurDate, obj.master.PurPoref, obj.master.PurRemarks, obj.detail.detail[i].PurRemarks, obj.detail.detail[i].PurQty, 0, 0, 0, obj.master.StrId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurNetAmt, obj.master.CurId, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, "Direct Purchase Order Detail", "UN-POST");

                            InventoryAccType = "";
                            TxsitemDetail item = new TxsitemDetail();
                            item = db.TxsitemDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ItmId == obj.detail.detail[i].ItmId).FirstOrDefault();
                            if (item != null)
                            {
                                if (item.ItmAssetAccount != null)
                                {
                                    InventoryAccType = item.ItmAssetAccount;
                                }
                                else if (item.ItmExpenseAccount != null && item.ItmIsService == false)
                                {
                                    InventoryAccType = item.ItmExpenseAccount;
                                }
                                else if (item.ItmCogsaccount != null && item.ItmIsService == true)
                                {
                                    InventoryAccType = item.ItmCogsaccount;
                                }

                                if (InventoryAccType != "")
                                {
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), InventoryAccType, "-1", 0, obj.detail.detail[i].PurGrossAmt, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, InventoryAccType, obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurAmount - obj.detail.detail[i].PurDiscountAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurAmount - obj.detail.detail[i].PurDiscountAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);

                                    if (obj.detail.detail[i].PurTaxAmt != null)
                                    {
                                        TxstaxDetail tax = new TxstaxDetail();
                                        tax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.detail.detail[i].TaxId).FirstOrDefault();
                                        if (tax != null)
                                        {
                                            if (tax.Coaid != null)
                                            {
                                                k = k + 1;
                                                tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, tax.TaxId.ToString(), obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurTaxAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurTaxAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                            }
                                        }
                                    }

                                    if (obj.detail.detail[i].PurExAmt != null)
                                    {
                                        TxstaxDetail excise = new TxstaxDetail();
                                        excise = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.detail.detail[i].ExciseId).FirstOrDefault();
                                        if (excise != null)
                                        {
                                            if (excise.Coaid != null)
                                            {
                                                k = k + 1;
                                                tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, excise.TaxId.ToString(), obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurTaxAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurTaxAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                            }
                                        }
                                    }

                                    if (obj.master.PurPayTerm == "CASH" || obj.master.PurPayTerm == "BANK ACCOUNT")
                                    {
                                        k = k + 1;
                                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.Coaid.ToString(), obj.master.SupId.ToString(), "-1", 0, obj.detail.detail[i].PurGrossAmt, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                        k = k + 1;
                                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.detail.detail[i].PurGrossAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                    }
                                }
                            }
                            ///////////
                            if (obj.master.PurPayTerm == "CREDIT" || obj.master.PurPayTerm == "BANK")
                            {
                                if (obj.TotalAmount != 0)
                                {
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.detail.detail[i].PurGrossAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                }
                            }
                            else if (obj.master.PurPayTerm == "CREDIT" && obj.Advance != 0 && obj.master.Coaid != null)
                            {
                                k = k + 1;
                                tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.Coaid.ToString(), obj.master.SupId.ToString(), "-1", 0, obj.Advance, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                k = k + 1;
                                tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.Advance, 0, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                k = k + 1;
                                tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", 0, obj.master.Coaid.ToString(), obj.master.Coaid.ToString(), "-1", 0, obj.Advance, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "TOTAL", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
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
                    if (count > 0)
                    {
                        TxtpurchaseMaster master = new TxtpurchaseMaster();
                        master = db.TxtpurchaseMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == obj.master.PurId).FirstOrDefault();
                        master.PurDcref = obj.master.PurDcref;
                        master.PurDate = obj.master.PurDate;
                        master.PurSupDate = obj.master.PurSupDate;
                        master.PurType = obj.master.PurType;
                        master.SupId = obj.master.SupId;
                        master.PurPayTerm = obj.master.PurPayTerm;
                        master.Coaid = obj.master.Coaid;
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

                        List<TxtpurchaseDetail> lstPurchase = new List<TxtpurchaseDetail>();
                        lstPurchase = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == obj.master.PurId).ToList();
                        for (int i = 0; i < lstPurchase.Count; i++)
                        {
                            db.TxtpurchaseDetail.Remove(lstPurchase[i]);
                            db.SaveChanges();
                        }

                        List<Txtledger> lstLedger = new List<Txtledger>();
                        lstLedger = db.Txtledger.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.Trno == obj.master.PurPoref).ToList();
                        for (int i = 0; i < lstLedger.Count; i++)
                        {
                            db.Txtledger.Remove(lstLedger[i]);
                            db.SaveChanges();
                        }

                        List<TxtinventoryStockDetail> lstInventoryStock = new List<TxtinventoryStockDetail>();
                        lstInventoryStock = db.TxtinventoryStockDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.InsNo == obj.master.PurPoref).ToList();
                        for (int i = 0; i < lstInventoryStock.Count; i++)
                        {
                            db.TxtinventoryStockDetail.Remove(lstInventoryStock[i]);
                            db.SaveChanges();
                        }
                        string InventoryAccType = ""; int k = 0;
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

                                tX.insertInventoryStockDetail(user.Id, user.UserName, obj.detail.detail[i].ItmId, i, obj.master.PurPoref, obj.master.PurDate, obj.master.PurPoref, obj.master.PurRemarks, obj.detail.detail[i].PurRemarks, obj.detail.detail[i].PurQty, 0, 0, 0, obj.master.StrId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurNetAmt, obj.master.CurId, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, "Direct Purchase Order Detail", "UN-POST");

                                InventoryAccType = "";
                                TxsitemDetail item = new TxsitemDetail();
                                item = db.TxsitemDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.ItmId == obj.detail.detail[i].ItmId).FirstOrDefault();
                                if (item != null)
                                {
                                    if (item.ItmAssetAccount != null)
                                    {
                                        InventoryAccType = item.ItmAssetAccount;
                                    }
                                    else if (item.ItmExpenseAccount != null && item.ItmIsService == false)
                                    {
                                        InventoryAccType = item.ItmExpenseAccount;
                                    }
                                    else if (item.ItmCogsaccount != null && item.ItmIsService == true)
                                    {
                                        InventoryAccType = item.ItmCogsaccount;
                                    }

                                    if (InventoryAccType != "")
                                    {
                                        k = k + 1;
                                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), InventoryAccType, "-1", 0, obj.detail.detail[i].PurGrossAmt, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                        k = k + 1;
                                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, InventoryAccType, obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurAmount - obj.detail.detail[i].PurDiscountAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurAmount - obj.detail.detail[i].PurDiscountAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);

                                        if (obj.detail.detail[i].PurTaxAmt != null)
                                        {
                                            TxstaxDetail tax = new TxstaxDetail();
                                            tax = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.detail.detail[i].TaxId).FirstOrDefault();
                                            if (tax != null)
                                            {
                                                if (tax.Coaid != null)
                                                {
                                                    k = k + 1;
                                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, tax.TaxId.ToString(), obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurTaxAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurTaxAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                                }
                                            }
                                        }

                                        if (obj.detail.detail[i].PurExAmt != null)
                                        {
                                            TxstaxDetail excise = new TxstaxDetail();
                                            excise = db.TxstaxDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.TaxId == obj.detail.detail[i].ExciseId).FirstOrDefault();
                                            if (excise != null)
                                            {
                                                if (excise.Coaid != null)
                                                {
                                                    k = k + 1;
                                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, excise.TaxId.ToString(), obj.master.SupId.ToString(), "-1", obj.detail.detail[i].PurTaxAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurTaxAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                                }
                                            }
                                        }

                                        if (obj.master.PurPayTerm == "CASH" || obj.master.PurPayTerm == "BANK ACCOUNT")
                                        {
                                            k = k + 1;
                                            tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.Coaid.ToString(), obj.master.SupId.ToString(), "-1", 0, obj.detail.detail[i].PurGrossAmt, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                            k = k + 1;
                                            tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.detail.detail[i].PurGrossAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                        }
                                    }
                                }
                                ///////////
                                if (obj.master.PurPayTerm == "CREDIT" || obj.master.PurPayTerm == "BANK")
                                {
                                    if (obj.TotalAmount != 0)
                                    {
                                        k = k + 1;
                                        tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.detail.detail[i].PurGrossAmt, 0, obj.master.PurExRate, obj.detail.detail[i].PurGrossAmt, obj.master.CurId, obj.detail.detail[i].TaxId, obj.detail.detail[i].PurTaxPer, obj.detail.detail[i].PurTaxAmt, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.detail.detail[i].PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                    }
                                }
                                else if (obj.master.PurPayTerm == "CREDIT" && obj.Advance != 0 && obj.master.Coaid != null)
                                {
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.Coaid.ToString(), obj.master.SupId.ToString(), "-1", 0, obj.Advance, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", k, obj.master.SupId.ToString(), obj.master.Coaid.ToString(), "-1", obj.Advance, 0, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "CHILD", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                    k = k + 1;
                                    tX.insertLedgerDetail(user.Id, user.UserName, obj.master.PurPoref, obj.master.PurDate, obj.master.PurDate, obj.master.PurPayTerm, "UN-POST", 0, obj.master.Coaid.ToString(), obj.master.Coaid.ToString(), "-1", 0, obj.Advance, obj.master.PurExRate, obj.Advance, obj.master.CurId, null, 0, 0, obj.master.PurChqNo, obj.master.PurChqDate, obj.master.PurPoref, obj.master.PurRemarks, "TOTAL", user.UserName, System.DateTime.Now, "Direct Purchase Order Detail", obj.master.SitId);
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("showPurchase");
        }


        public IActionResult PrintAllPurchase(string id)
        {

            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            TXTPurchaseDetailView lstPurchase = new TXTPurchaseDetailView();
            lstPurchase.lstMaster = db.TxtpurchaseMaster.Where(x => x.Id == UId && x.UserName == UName).ToList();
            lstPurchase.detail.detail = db.TxtpurchaseDetail.Where(x => x.Id == UId && x.UserName == UName).OrderBy(x => x.PurSerialNo).ToList();
            lstPurchase.lstBank = db.TxsbankDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstCurrency = db.TxscurrencyDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstStore = db.TxsstoreDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstSite = db.TxssiteDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstItem = db.TxsitemDetail.Where(x => x.UserName == UName).ToList();
            lstPurchase.lstTax = db.TxstaxDetail.Where(x => x.UserName == UName && x.TaxType == "PURCHASE" && x.TaxActive == true).ToList();
            lstPurchase.lstExcise = db.TxstaxDetail.Where(x => x.UserName == UName && x.TaxType == "SALE" && x.TaxActive == true).ToList();
            for (int i = 0; i < lstPurchase.detail.detail.Count; i++)
            {
                PDEF pDEF = new PDEF();
                TXSItemUOMDetail item = new TXSItemUOMDetail();
                int itmid = (int)lstPurchase.detail.detail[i].ItmId;
                if (itmid != -1)
                {
                    item = changeItems(itmid, UId, UName);
                    pDEF.UOM = item.Txuom.Uomname;
                    pDEF.lastPrice = item.Txsitem.ItmSp;
                }

                pDEF.subAmount = lstPurchase.detail.detail[i].PurQty * lstPurchase.detail.detail[i].PurRate;
                pDEF.AmtAfterExcise = pDEF.subAmount + lstPurchase.detail.detail[i].PurExAmt;
                pDEF.AmtAfterDiscount = pDEF.subAmount + lstPurchase.detail.detail[i].PurExAmt - lstPurchase.detail.detail[i].PurDiscountAmt;
                lstPurchase.detail.pdef.Add(pDEF);
                lstPurchase.detail.detail[i].PurGrossAmt = lstPurchase.detail.detail[i].PurNetAmt * lstPurchase.master.PurExRate;
            }

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportAllPurchase", lstPurchase)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

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
                    saleDetail sale = new saleDetail();
                    TXSItemUOMDetail item = new TXSItemUOMDetail();
                    int itmid = (int)obj.Detail.saleDetail[i].ItmId;
                    if (itmid != -1)
                    {
                        item = changeItems(itmid, user.Id, user.UserName);
                        sale.UOM = item.Txuom.Uomname;
                        sale.LastPrice = item.Txsitem.ItmSp;
                        sale.BarCode = item.Txsitem.ItmBcode;
                    }

                    sale.Amount = obj.Detail.saleDetail[i].SalQty * obj.Detail.saleDetail[i].SalRate;
                    sale.AmtAfterExcise = sale.Amount + obj.Detail.saleDetail[i].SalExAmt;
                    sale.AmtAfterDiscount = sale.Amount + obj.Detail.saleDetail[i].SalExAmt - obj.Detail.saleDetail[i].SalDiscAmt;
                    obj.Detail.sale.Add(sale);
                    obj.Detail.saleDetail[i].SalGrossAmt = obj.Detail.saleDetail[i].SalNetAmt * obj.master.SalExRate;
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
            if (Save != null)
            {
                int count = 0;
                for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                {
                    if (obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
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

                    db.TxtsaleMaster.Add(obj.master);
                    db.SaveChanges();

                    for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                    {
                        if (obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
                        {
                            obj.Detail.saleDetail[i].Id = user.Id;
                            obj.Detail.saleDetail[i].UserName = user.UserName;
                            obj.Detail.saleDetail[i].SalId = obj.master.SalId;

                            db.TxtsaleDetail.Add(obj.Detail.saleDetail[i]);
                            db.SaveChanges();
                        }
                    }
                }
            }
            if (Update != null)
            {
                int count = 0;
                for (int i = 0; i < obj.Detail.saleDetail.Count; i++)
                {
                    if (obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    TxtsaleMaster master = new TxtsaleMaster();
                    master = db.TxtsaleMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.SalId == obj.master.SalId).FirstOrDefault();
                    master.SalLporef = obj.master.SalLporef;
                    master.SalDate = obj.master.SalDate;
                    master.SalSupplyDate = obj.master.SalSupplyDate;
                    master.SalType = obj.master.SalType;
                    master.CusId = obj.master.CusId;
                    master.SalPayTerms = obj.master.SalPayTerms;
                    master.Coaid = obj.master.Coaid;
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
                        if (obj.Detail.saleDetail[i].ItmId != -1 && obj.Detail.saleDetail[i].SalQty > 0 && obj.Detail.saleDetail[i].SalRate > 0)
                        {
                            obj.Detail.saleDetail[i].Id = user.Id;
                            obj.Detail.saleDetail[i].UserName = user.UserName;
                            obj.Detail.saleDetail[i].SalId = obj.master.SalId;
                            db.TxtsaleDetail.Add(obj.Detail.saleDetail[i]);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("showSale");
        }



        #endregion

        #region Credit Note

        [HttpGet]
        public async Task<IActionResult> showCreditNote()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTCreditNoteDetailView creditnote = new TXTCreditNoteDetailView();
            creditnote.lstCustomer = db.TxscustomerDetail.Where(x => x.UserName == user.UserName).ToList();
            creditnote.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            creditnote.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            creditnote.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            creditnote.lstMaster = db.TxtcreditNoteMaster.Where(x => x.UserName == user.UserName).ToList();
            creditnote.detail.detail = db.TxtcreditNoteDetail.Where(x => x.UserName == user.UserName).ToList();
            return View(creditnote);
        }

        [HttpGet]
        public async Task<IActionResult> CreditNote(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            TXTCreditNoteDetailView lstcreditNote = new TXTCreditNoteDetailView();
            lstcreditNote.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            lstcreditNote.lstCustomer = db.TxscustomerDetail.Where(x => x.UserName == user.UserName).ToList();
            lstcreditNote.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            lstcreditNote.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                lstcreditNote.master.ScnRefNo = tX.CreditNote(user.UserName);
                lstcreditNote.detail.detail = null;
                lstcreditNote.detail.cndef = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";

                //lstcreditNote.master = db.TxtpurchaseMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).FirstOrDefault();
                //lstcreditNote.detail.detail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).OrderBy(x => x.PurSerialNo).ToList();
                //for (int i = 0; i < obj.detail.detail.Count; i++)
                //{
                //    PDEF pDEF = new PDEF();
                //    TXSItemUOMDetail item = new TXSItemUOMDetail();
                //    int itmid = (int)obj.detail.detail[i].ItmId;
                //    if (itmid != -1)
                //    {
                //        item = changeItems(itmid, user.Id, user.UserName);
                //        pDEF.UOM = item.Txuom.Uomname;
                //        pDEF.lastPrice = item.Txsitem.ItmSp;
                //    }

                //    pDEF.subAmount = obj.detail.detail[i].PurQty * obj.detail.detail[i].PurRate;
                //    pDEF.AmtAfterExcise = pDEF.subAmount + obj.detail.detail[i].PurExAmt;
                //    pDEF.AmtAfterDiscount = pDEF.subAmount + obj.detail.detail[i].PurExAmt - obj.detail.detail[i].PurDiscountAmt;
                //    obj.detail.pdef.Add(pDEF);
                //    obj.detail.detail[i].PurGrossAmt = obj.detail.detail[i].PurNetAmt * obj.master.PurExRate;
                //
            }
            return View(lstcreditNote);
        }

        #endregion

        #region Debit Note

        [HttpGet]
        public async Task<IActionResult> showDebitNote()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTDebitNoteDetailView debitnote = new TXTDebitNoteDetailView();
            debitnote.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == user.UserName).ToList();
            debitnote.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            debitnote.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            debitnote.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            debitnote.lstMaster = db.TxtdebitNoteMaster.Where(x => x.UserName == user.UserName).ToList();
            debitnote.detail.detail = db.TxtdebitNoteDetail.Where(x => x.UserName == user.UserName).ToList();
            return View(debitnote);
        }

        [HttpGet]
        public async Task<IActionResult> DebitNote(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            TXTDebitNoteDetailView lstdebitNote = new TXTDebitNoteDetailView();
            lstdebitNote.lstStore = db.TxsstoreDetail.Where(x => x.UserName == user.UserName).ToList();
            lstdebitNote.lstSupplier = db.TxssupplierDetail.Where(x => x.UserName == user.UserName).ToList();
            lstdebitNote.lstSite = db.TxssiteDetail.Where(x => x.UserName == user.UserName).ToList();
            lstdebitNote.lstItem = db.TxsitemDetail.Where(x => x.UserName == user.UserName).ToList();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";
                lstdebitNote.master.PdnRefNo = tX.DebitNote(user.UserName);
                lstdebitNote.detail.detail = null;
                lstdebitNote.detail.dndef = null;
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";

                //lstcreditNote.master = db.TxtpurchaseMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).FirstOrDefault();
                //lstcreditNote.detail.detail = db.TxtpurchaseDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PurId == Convert.ToInt32(id)).OrderBy(x => x.PurSerialNo).ToList();
                //for (int i = 0; i < obj.detail.detail.Count; i++)
                //{
                //    PDEF pDEF = new PDEF();
                //    TXSItemUOMDetail item = new TXSItemUOMDetail();
                //    int itmid = (int)obj.detail.detail[i].ItmId;
                //    if (itmid != -1)
                //    {
                //        item = changeItems(itmid, user.Id, user.UserName);
                //        pDEF.UOM = item.Txuom.Uomname;
                //        pDEF.lastPrice = item.Txsitem.ItmSp;
                //    }

                //    pDEF.subAmount = obj.detail.detail[i].PurQty * obj.detail.detail[i].PurRate;
                //    pDEF.AmtAfterExcise = pDEF.subAmount + obj.detail.detail[i].PurExAmt;
                //    pDEF.AmtAfterDiscount = pDEF.subAmount + obj.detail.detail[i].PurExAmt - obj.detail.detail[i].PurDiscountAmt;
                //    obj.detail.pdef.Add(pDEF);
                //    obj.detail.detail[i].PurGrossAmt = obj.detail.detail[i].PurNetAmt * obj.master.PurExRate;
                //
            }
            return View(lstdebitNote);
        }

        public async Task<IActionResult> DebitNote(TXTDebitNoteDetailView obj, string Save, string Update)
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
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxtdebitNoteMaster.Add(obj.master);
                db.SaveChanges();


                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    if (obj.detail.detail[i].ItmId != -1 && obj.detail.detail[i].PdnDeliveredQuantity > 0)
                    {
                        obj.detail.detail[i].PdnId = obj.master.PdnId;
                        obj.detail.detail[i].Id = user.Id;
                        obj.detail.detail[i].UserName = user.UserName;
                        db.TxtdebitNoteDetail.Add(obj.detail.detail[i]);
                        db.SaveChanges();


                    }
                }
            }
            if (Update != null)
            {
                var count = 0;
                for (int i = 0; i < obj.detail.detail.Count; i++)
                {
                    if (obj.detail.detail[i].ItmId != -1 && obj.detail.detail[i].PdnDeliveredQuantity > 0)
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    TxtdebitNoteMaster DNM = new TxtdebitNoteMaster();
                    DNM = db.TxtdebitNoteMaster.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PdnId == obj.master.PdnId).FirstOrDefault();
                    DNM.PdnCode = obj.master.PdnCode;
                    DNM.PdnDate = obj.master.PdnDate;
                    DNM.PdnGpno = obj.master.PdnGpno;
                    DNM.PdnGpdate = obj.master.PdnGpdate;
                    DNM.PdnCondition = obj.master.PdnCondition;
                    DNM.PdnLocType = obj.master.PdnLocType;
                    DNM.StrId = obj.master.StrId;
                    DNM.WrhId = obj.master.WrhId;
                    DNM.ShtId = obj.master.ShtId;
                    DNM.PdnPurchaseType = obj.master.PdnPurchaseType;
                    DNM.PdnPurchaseCode = obj.master.PdnPurchaseCode;
                    DNM.PdnRefNo = obj.master.PdnRefNo;
                    DNM.PdnRemarks = obj.master.PdnRemarks;
                    DNM.GrpId = obj.master.GrpId;
                    DNM.ComId = obj.master.ComId;
                    DNM.EditBy = user.UserName;
                    DNM.EditDate = System.DateTime.Now;
                    DNM.PdnSubAmtTotalMain = obj.master.PdnSubAmtTotalMain;
                    DNM.PdnTotalDiscMain = obj.master.PdnTotalDiscMain;
                    DNM.PdnTotalTaxMain = obj.master.PdnTotalTaxMain;
                    DNM.PdnTotalPaid = obj.master.PdnTotalPaid;
                    DNM.PdnTotalBalance = obj.master.PdnTotalBalance;
                    DNM.PdnTotalAmountMain = obj.master.PdnTotalAmountMain;
                    DNM.PdnBillNo = obj.master.PdnBillNo;
                    DNM.PdnTotalExciseTaxMain = obj.master.PdnTotalExciseTaxMain;
                    DNM.PdnPurchaseTrNo = obj.master.PdnPurchaseTrNo;
                    DNM.PdnPurchaseDate = obj.master.PdnPurchaseDate;
                    DNM.AccId = obj.master.AccId;
                    DNM.SitId = obj.master.SitId;
                    db.SaveChanges();

                    List<TxtdebitNoteDetail> lstDebitNote = new List<TxtdebitNoteDetail>();
                    lstDebitNote = db.TxtdebitNoteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.PdnId == obj.master.PdnId).OrderBy(x => x.PdnSerialNo).ToList();
                    for (int i = 0; i < lstDebitNote.Count; i++)
                    {
                        db.TxtdebitNoteDetail.Remove(lstDebitNote[i]);
                        db.SaveChanges();
                    }

                    for (int i = 0; i < obj.detail.detail.Count; i++)
                    {
                        if (obj.detail.detail[i].ItmId != -1 && obj.detail.detail[i].PdnDeliveredQuantity > 0)
                        {
                            obj.detail.detail[i].PdnId = obj.master.PdnId;
                            obj.detail.detail[i].Id = user.Id;
                            obj.detail.detail[i].UserName = user.UserName;
                            db.TxtdebitNoteDetail.Add(obj.detail.detail[i]);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return View();
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

        public string purchaseTotalAmount(string RefCode)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            return db.Txtledger.Where(x => x.Id == id && x.UserName == userName && x.Trno == RefCode).Sum(x => x.Trdebit - x.Trcredit).ToString();
        }

        public TXSItemUOMDetail changeItems(int id, string userid, string userName)
        {
            TXSItemUOMDetail obj = new TXSItemUOMDetail();
            obj.Txsitem = db.TxsitemDetail.Where(x => x.UserName == userName && x.Id == userid && x.ItmId == id).OrderByDescending(x => x.ItmId).FirstOrDefault();
            obj.Txuom = db.Txsuomdetail.Where(x => x.Uomid == Convert.ToInt32(obj.Txsitem.ItmUom)).FirstOrDefault();
            return obj;
        }

        public async Task<TxstaxDetail> changeTax(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            TxstaxDetail obj = new TxstaxDetail();
            obj = db.TxstaxDetail.Where(x => x.UserName == user.UserName && x.TaxId == Convert.ToInt32(id)).FirstOrDefault();
            return obj;
        }


        public List<Txscoadetail> changeTerm(string Term)
        {
            string id = HttpContext.Session.GetString("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            List<Txscoadetail> lstAccount = new List<Txscoadetail>();
            if (Term == "CREDIT") { lstAccount = db.Txscoadetail.Where(x => x.Id==id && x.UserName == userName && (x.AccAccountSubNature.Contains("CASH") || x.AccAccountSubNature.Contains("BANK ACCOUNT")) && x.AccActive == true).ToList(); }
            else { lstAccount = db.Txscoadetail.Where(x => x.Id == id && x.UserName == userName && x.AccAccountSubNature.Contains(Term) && x.AccActive == true).ToList(); }

            return lstAccount;
        }


        #endregion
    }
}
