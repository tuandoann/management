using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Common;
namespace QUANLYTIEC.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string nameUser, string pass)
        {
            if (string.IsNullOrWhiteSpace(nameUser) || string.IsNullOrWhiteSpace(pass))
                return View();
           SYS_USER item = DA_User.Instance.getUserBaseNameAndPass(nameUser, Encrypt.MD5Hash(pass));
          

            if (item !=null)
            {
                Session["UserID"] = item.UserID;
                Session["UserName"] = item.UserName;
                Session["FullName"] = item.FullName;
                Session["RoleID"] = item.RoleID == null ? 0 : item.RoleID;
                Session["UserImage"] = null;
                Session["IsAdmin"] = item.IsAdmin;
                Session["IsConfig"] = item.IsConfig;
                return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.result = 1;
            return View();
        }
        public ActionResult LogOut()
        {

            // set some session to null
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["FullName"] = null;
            Session["RoleID"] = null;
            Session["UserImage"] = null;
            Session["IsAdmin"] = null;

            return RedirectToAction("Index","Login");
        }
	}
}