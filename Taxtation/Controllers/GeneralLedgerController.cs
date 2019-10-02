using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Taxtation.Controllers
{
    public class GeneralLedgerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}