using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Models;
using System.Transactions;

namespace QUANLYTIEC.Controllers
{
    public class DepartmentController : BaseController
    {
        #region method is used for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_DEPARTMENT).Name, "index");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_DEPARTMENT).Name, "edit");
                return View(DA_Department.Instance.GetById(Convert.ToInt32(id)));
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_DEPARTMENT).Name, "index");
            return RedirectToAction("Index", "Department");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_DEPARTMENT).Name, "create");
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDepartmentDatatableIndex()
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
                var sortColumn = Request.Form.GetValues("columns[" + orderColumn + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_Department.Instance.getDepartmentmForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_Department.Instance.countAllDepartmentFlowSearch(search.ToString());
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
                    return Json(DA_Department.Instance.deleteEntity(Convert.ToInt32(id)));
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        /// <summary>
        /// ajax add or update entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOrUpdateEntity(TBL_DEPARTMENT item, bool isEdit)
        {
            if (!string.IsNullOrWhiteSpace(item.DepartmentName) && ((isEdit && item.DepartmentID > 0) || !isEdit))
            {
                try
                {
                    int result = isEdit ? DA_Department.Instance.Update(item) : DA_Department.Instance.Insert(item);
                    return Json(result > 0 ? 1 : 0);
                }
                catch (Exception ex) { }
            }
            return Json(0);
        }
        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach(int id in lsIdItem)
                    {
                        DA_Department.Instance.deleteEntity(id);
                    }
                    scope.Complete();
                    return Json(1);
                }
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        #endregion
    }
}