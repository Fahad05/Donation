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
    public class DonationController : Controller
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
        public DonationController(
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

        #region Donation

        [HttpGet]
        public async Task<IActionResult> showDonation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            List<TxtdonationDetail> lstDonation = new List<TxtdonationDetail>();
            lstDonation = db.TxtdonationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName).ToList();
            return View(lstDonation);
        }

        [HttpGet]
        public async Task<IActionResult> Donation(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            TXTDonationDetailView obj = new TXTDonationDetailView();
            if (id == null)
            {
                ViewData["_Save"] = "True";
                ViewData["_Update"] = "False";

                obj.lstDonor = db.TxsdonorDetail.ToList();
                obj.lstCategory = db.TxsdonationCategoryDetail.ToList();
                obj.master.DnnActive = (obj.master.DnnActive == null) ? true : false;
                obj.master.DnnTaxable = (obj.master.DnnTaxable == null) ? false : true;
                return PartialView(obj);
            }
            else
            {
                ViewData["_Save"] = "False";
                ViewData["_Update"] = "True";
                obj.master = db.TxtdonationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DnnId == Convert.ToInt32(id)).FirstOrDefault();
                obj.lstDonor = db.TxsdonorDetail.Where(x => x.DnrId == obj.master.DnnDnrId).ToList();
                obj.lstCategory = db.TxsdonationCategoryDetail.Where(x => x.DcaId == obj.master.DnnDcaId).ToList();
                obj.master.DnnActive = (obj.master.DnnActive == true) ? true : false;
                obj.master.DnnTaxable = (obj.master.DnnTaxable == true) ? true : false;
                return PartialView(obj);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Donation(TXTDonationDetailView obj, string DnnActive, string DnnTaxable, string Save, string Update)
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
                obj.master.DnnActive = (DnnActive == "true") ? true : false;
                obj.master.DnnTaxable = (DnnTaxable == "true") ? true : false;
                obj.master.EnterBy = user.UserName;
                obj.master.EnterDate = System.DateTime.Now;
                db.TxtdonationDetail.Add(obj.master);
                db.SaveChanges();
            }
            if (Update != null)
            {
                TxtdonationDetail obj1 = new TxtdonationDetail();
                obj1 = db.TxtdonationDetail.Where(x => x.Id == user.Id && x.UserName == user.UserName && x.DnnId == obj.master.DnnId).FirstOrDefault();
                if (obj1 != null)
                {
                    obj1.DnnDnrId = obj.master.DnnDnrId;
                    obj1.DnnAmount = obj.master.DnnAmount;
                    obj1.DnnType = obj.master.DnnType;
                    obj1.DnnChequeNo = obj.master.DnnChequeNo;
                    obj1.DnnDate = obj.master.DnnDate;
                    obj1.DnnDcaId = obj.master.DnnDcaId;
                    obj1.DnnDesc = obj.master.DnnDesc;
                    obj1.DnnActive = (DnnActive == "true") ? true : false;
                    obj1.DnnTaxable = (DnnTaxable == "true") ? true : false;
                    obj1.EditBy = user.UserName;
                    obj1.EditDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("showDonation");
        }

        #endregion

    }
}