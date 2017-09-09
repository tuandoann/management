using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.Transactions;

namespace QUANLYTIEC.Controllers
{
    public class UOMController : BaseController
    {
        #region method is used for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_UOM).Name, "index");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_UOM).Name, "edit");
                return View(DA_UOM.Instance.GetById(Convert.ToInt32(id)));
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_UOM).Name, "index");
            return RedirectToAction("Index", "UOM");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_UOM).Name, "create");
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGroupFoodDatatableIndex()
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

                List<object> data = DA_UOM.Instance.getUomForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_UOM.Instance.countAllUomFlowSearch(search.ToString());
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
                    TBL_UOM uom = DA_UOM.Instance.GetById(Convert.ToInt32(id));
                    return Json(uom == null ? 0 : DA_UOM.Instance.Delete(uom));
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        #endregion

        #region method is used for sumbit form
        /// <summary>
        /// method excute save item when sumbit form create
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string uOMName)
        {
            if (!string.IsNullOrWhiteSpace(uOMName))
            {
                try
                {
                    TBL_UOM item = new TBL_UOM();
                    item.UOMName = uOMName;
                    DA_UOM.Instance.Insert(item);
                    return RedirectToAction("Index", "UOM");
                }
                catch (Exception ex) { }
            }
            return View();
        }
        /// <summary>
        /// method excute save item when sumbit form edit
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="serviceName"></param>
        /// <param name="unitPrice"></param>
        /// <param name="notes"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string uOMID, string uOMName)
        {
            if (!string.IsNullOrWhiteSpace(uOMName) && !string.IsNullOrWhiteSpace(uOMID) && uOMID.All(Char.IsDigit))
            {
                try
                {
                    TBL_UOM item = DA_UOM.Instance.GetById(Convert.ToInt32(uOMID));
                    item.UOMName = uOMName;
                    DA_UOM.Instance.Update(item);
                    return RedirectToAction("Index", "UOM");
                }
                catch (Exception ex) { return View(); }
            }
            return View();
        }
        #region delete

        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var id in lsIdItem)
                    {
                        DA_UOM.Instance.deleteEntity(Convert.ToInt32(id));
                    }
                    scope.Complete();
                    return Json(1);
                }

            }
            catch (Exception ex) { return Json(ex.Message); }

        }
        #endregion

        #endregion
    }
}