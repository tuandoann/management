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
    public class ServiceController : BaseController
    {

        #region method is used for load page
        /// <summary>
        /// load page index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_SERVICE).Name, "index");
            return View();
        }
        /// <summary>
        /// load page edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_SERVICE).Name, "edit");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                return View(DA_Service.Instance.GetById(Convert.ToInt32(id)));
            }
            return RedirectToAction("Index", "Service");
        }
        /// <summary>
        /// load page create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_SERVICE).Name, "create");
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetServiceDatatableIndex()
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

                List<object> data = DA_Service.Instance.getServiceForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_Service.Instance.countAllServiceFlowSearch(search.ToString());
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
                    TBL_SERVICE user = DA_Service.Instance.GetById(Convert.ToInt32(id));
                    return Json(user == null ? 0 : DA_Service.Instance.Delete(user));
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
            [HttpPost]
        public JsonResult GetUnitPrice()
        {
            try
            {
                //jQuery DataTables Param
                string id = (Request.Form.GetValues("id").FirstOrDefault() == null
                    ? ""
                    : Request.Form.GetValues("id").FirstOrDefault().ToString());
                if (id.All(Char.IsDigit))
                {
                    TBL_SERVICE item = DA_Service.Instance.GetById(Convert.ToInt32(id));
                    return Json(item == null ? 1 : item.UnitPrice);
                }
            }
            catch (Exception ex) {}
            return Json(1);
        }
        #endregion

        #region method is used for sumbit form
        /// <summary>
        /// method excute save item when sumbit form create
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="unitPrice"></param>
        /// <param name="notes"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string serviceName, string unitPrice, string notes, string isActive)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                try
                {
                    unitPrice = !unitPrice.All(Char.IsDigit) && string.IsNullOrWhiteSpace(unitPrice) ? "0" : unitPrice;
                    TBL_SERVICE item = new TBL_SERVICE();
                    item.ServiceName = serviceName;
                    item.UnitPrice = Convert.ToDecimal(unitPrice);
                    item.Notes = notes;
                    item.IsActive = (isActive == "on") ? true : false;
                    DA_Service.Instance.Insert(item);
                    return RedirectToAction("Index", "Service");
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
        public ActionResult Edit(string serviceID, string serviceName, string unitPrice, string notes, string isActive)
        {
            if (!string.IsNullOrWhiteSpace(serviceID) && !string.IsNullOrWhiteSpace(serviceName) && serviceID.All(Char.IsDigit))
            {
                try
                {
                    unitPrice = !unitPrice.All(Char.IsDigit) && string.IsNullOrWhiteSpace(unitPrice) ? "0" : unitPrice;
                    TBL_SERVICE item = DA_Service.Instance.GetById(Convert.ToInt32(serviceID));
                    item.ServiceName = serviceName;
                    item.UnitPrice = Convert.ToDecimal(unitPrice);
                    item.Notes = notes;
                    item.IsActive = (isActive == "on") ? true : false;
                    DA_Service.Instance.Update(item);
                    return RedirectToAction("Index", "Service");
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
                        DA_Service.Instance.deleteEntity(Convert.ToInt32(id));
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