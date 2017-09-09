using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Models;
using System.Security.Cryptography;
using QUANLYTIEC.Common;

namespace QUANLYTIEC.Controllers
{
    public class UserController : BaseController
    {
        //
        #region method is used for load page
        /// <summary>
        /// load page index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(SYS_USER).Name, "index");
            return View();
        }
        /// <summary>
        /// load page create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(SYS_USER).Name, "create");
            return View();
        }
        /// <summary>
        /// load page change password
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        /// <summary>
        /// load page edit
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(SYS_USER).Name, "edit");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {             
                var item = DA_User.Instance.GetById(Convert.ToInt32(id));
               
             //  ViewBag.pass =  Encrypt.Decrypt(item.Password);

                return View(item);
            }
            return RedirectToAction("Index","User");
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserDatatableIndex()
        {
            try
            {
                #region get para from view
                //jQuery DataTables Param
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                //Find paging info
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault());
                //Find order columns info
                var sortColumn = Request.Form.GetValues("columns[" + (orderColumn == 0 ? 1 : orderColumn) + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_User.Instance.getUserForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_User.Instance.countAllUserFlowSearch(search.ToString());
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        /// <summary>
        /// ajax delete item
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult delete()
        {
            try
            {
                //jQuery DataTables Param
                string id = (Request.Form.GetValues("id").FirstOrDefault() == null
                    ? ""
                    : Request.Form.GetValues("id").FirstOrDefault().ToString());
                if (id.All(Char.IsDigit))
                {
                    SYS_USER user = DA_User.Instance.GetById(Convert.ToInt32(id));
                    return Json(user == null ? 0 : DA_User.Instance.Delete(user));
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        /// <summary>
        /// ajax check user exist
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult checkExistUser()
        {
            string userName = (Request.Form.GetValues("userName").FirstOrDefault() == null ? "" : Request.Form.GetValues("userName").FirstOrDefault().ToString());
            if(!string.IsNullOrWhiteSpace(userName))
            {
                return Json(DA_User.Instance.getItemBasetUserName(userName) != null ? 1 : 0);
            }
            return Json(0);
        }
        /// <summary>
        /// ajax add or update entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOrUpdateEntity(SYS_USER item, bool isEdit)
        {
            //if (!string.IsNullOrWhiteSpace(item.DepartmentName) && ((isEdit && item.DepartmentID > 0) || !isEdit))
            //{
                try
                {
                    item.Password =  Encrypt.MD5Hash(item.Password);
                    int result = isEdit ? DA_User.Instance.Update(item) : DA_User.Instance.Insert(item);
                    return Json(result > 0 ? 1 : 0);
                }
                catch (Exception ex) { }
            //}
            return Json(0);
        }
        /// <summary>
        /// ajax add or update entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangePassWord(string passwordOld, string passworldNew)
        {
            try
            {
                passwordOld = Encrypt.MD5Hash(passwordOld);
                passworldNew = Encrypt.MD5Hash(passworldNew);
                int idCurrentUser = Convert.ToInt32(Session["UserID"]);
                SYS_USER userCurrent = DA_User.Instance.GetById(idCurrentUser);
                if(userCurrent.Password == passwordOld)
                {
                    userCurrent.Password = passworldNew;
                    return Json(DA_User.Instance.Update(userCurrent) > 0 ? 1 : 0);
                }
                else
                {
                    throw new Exception("Mật khẩu hiện tại không đúng. Xin vui lòng nhập lại!");
                }
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        #endregion

        #region method is used for sumbit form

        [HttpPost]
        public ActionResult Create(string userName, string pass, string fullName, string active, string email)
        {
            ViewBag.title = TitleEnum.getTitleForPage(typeof(SYS_USER).Name, "create");
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(pass) && 
                !string.IsNullOrWhiteSpace(fullName))
            {
                if (DA_User.Instance.getUserBaseNameAndPass(userName, pass) == null)
                {
                    try
                    {
                        SYS_USER item = new SYS_USER();
                        item.UserName = userName;
                        item.Password = Encrypt.MD5Hash(pass);
                        item.FullName = fullName;
                        item.IsActive = (active == "on") ? true : false;
                        item.Email = email;
                        item.IsAdmin = false;
                        DA_User.Instance.Insert(item);
                        return RedirectToAction("Index", "User");
                    }
                    catch (Exception ex) { }
                }
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Edit(string userId, string userName, string pass, string fullName, string active, string email)
        {
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(pass) &&
                !string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(userId) && userId.All(Char.IsDigit))
            {
                //user id ís Exist
                SYS_USER user = DA_User.Instance.GetById(Convert.ToInt32(userId));
                if (user != null)
                {
                    try
                    {
                        SYS_USER checkNameAndPass = DA_User.Instance.getItemBasetUserName(userName);
                        //check user name and pass Exist another user or this user not Exist
                        if (checkNameAndPass.UserID == user.UserID || checkNameAndPass == null)
                        {
                            user.UserName = userName;
                            user.Password = Encrypt.MD5Hash(pass);
                            user.FullName = fullName;
                            user.IsActive = (active == "on") ? true : false;
                            user.Email = email;
                            DA_User.Instance.Update(user);
                            return RedirectToAction("Index", "User");
                        }
                        ViewBag.title = TitleEnum.getTitleForPage(typeof(SYS_USER).Name, "edit");
                    }
                    catch (Exception ex) { }
                }
                return View();
            }
            return View();
        }
        #endregion
    }
}