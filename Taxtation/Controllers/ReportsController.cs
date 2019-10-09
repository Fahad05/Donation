using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Taxtation.Models;
using Taxtation.ViewModel;

namespace Taxtation.Controllers
{
    public class ReportsController : Controller
    {
        static string UId;
        static string UName;
        TAXTATIONContext db = new TAXTATIONContext();

        #region Setup Reports
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
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

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



        public IActionResult PrintAllCOA()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (User == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            List<Txscoadetail> lstCoa = new List<Txscoadetail>();
            lstCoa = db.Txscoadetail.ToList();

            //lstSite = db.TxssiteDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return new ViewAsPdf("ReportCOA", lstCoa)
            {
                // CustomSwitches = "--page-offset 0 --footer-center Page: [page]/[toPage]\ --footer-font-size 12"};
                CustomSwitches = "--footer-center \"  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
            };

        }
        #endregion

        #region Transaction Reports
        public TXSItemUOMDetail changeItems(int id, string userid, string userName)
        {
            TXSItemUOMDetail obj = new TXSItemUOMDetail();
            obj.Txsitem = db.TxsitemDetail.Where(x => x.UserName == userName && x.Id == userid && x.ItmId == id).OrderByDescending(x => x.ItmId).FirstOrDefault();
            obj.Txuom = db.Txsuomdetail.Where(x => x.Uomid == Convert.ToInt32(obj.Txsitem.ItmUom)).FirstOrDefault();
            return obj;
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
    }
}