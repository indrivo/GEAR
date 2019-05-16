using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ST.Cms.Controllers
{
    public class FIlesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}