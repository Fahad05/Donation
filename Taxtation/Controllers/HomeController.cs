using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taxtation.Models;

namespace Taxtation.Controllers
{
    public class HomeController : Controller
    {
        TAXTATIONContext db = new TAXTATIONContext();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Setup()
        {
            return View();
        }

        public IActionResult Sale()
        {
            return View();
        }

        public IActionResult Purchase()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Ledger()
        {
            return View();
        }

        public IActionResult Inventory()
        {
            return View();
        }

        public JsonResult CurrencyChart()
        {
            List<TxscurrencyDetail> lstCurrency = new List<TxscurrencyDetail>();
            lstCurrency = db.TxscurrencyDetail.ToList();

            return Json(lstCurrency);
        }
    }
}
