using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;

namespace QUANLYTIEC.Controllers
{
    public class MenuController : Controller
    {
        [HttpPost]
        public JsonResult GetMenuForWeb()
        {
            //   int UserID = int.Parse(Session["UserID"].ToString());

            return Json(DA_Function.Instance.GetAll().ToList());



        }
    }
}