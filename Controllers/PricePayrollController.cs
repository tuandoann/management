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
    public class PricePayrollController : BaseController
    {
        #region method is used for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRICE_PAYROLL).Name, "index");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRICE_PAYROLL).Name, "edit");
                return View(DA_PricePayroll.Instance.GetById(Convert.ToInt32(id)));
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRICE_PAYROLL).Name, "index");
            return RedirectToAction("Index", "PricePayroll");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRICE_PAYROLL).Name, "create");
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPricePayrollDatatableIndex()
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
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_PricePayroll.Instance.getPricePayrollForDatatablePagging(skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_PricePayroll.Instance.countAllPricePayrollFlowSearch();
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
                    TBL_PRICE_PAYROLL pricePayroll = DA_PricePayroll.Instance.GetById(Convert.ToInt32(id));
                    return Json(pricePayroll == null ? 0 : DA_PricePayroll.Instance.Delete(pricePayroll));
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
        public JsonResult AddOrUpdateEntity(TBL_PRICE_PAYROLL item, bool isEdit, string TimeFrom, string TimeTo)
        {
            DateTime timeFrom = new DateTime(2000, 1, 1, Convert.ToInt32(TimeFrom.Split('/')[0]), Convert.ToInt32(TimeFrom.Split('/')[1]), 0);
            DateTime timeTo = new DateTime(2000, 1, 1, Convert.ToInt32(TimeTo.Split('/')[0]), Convert.ToInt32(TimeTo.Split('/')[1]), 0);
            if (DateTime.Compare(timeTo, timeFrom) >= 0)
            {
                try
                {
                    item.TimeFrom = timeFrom;
                    item.TimeTo = timeTo;
                    int result = isEdit ? DA_PricePayroll.Instance.Update(item) : DA_PricePayroll.Instance.Insert(item);
                    return Json(result > 0 ? 1 : 0);
                }
                catch (Exception ex) { }
            }
            else
            {
                return Json(2);
            }
            return Json(0);
        }



        #endregion

        #region  delete
        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var id in lsIdItem)
                    {
                        DA_PricePayroll.Instance.deleteEntity(Convert.ToInt32(id));
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