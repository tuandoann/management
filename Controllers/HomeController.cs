using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QUANLYTIEC.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
    }
}