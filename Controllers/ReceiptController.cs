using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Models;
using System.Globalization;
using System.Transactions;

namespace QUANLYTIEC.Controllers
{
    public class ReceiptController : BaseController
    {
        #region method for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(char.IsDigit))
            {

                ViewBag.comboboxPT = DA_Party.Instance.GetAll().OrderByDescending(x => x.PartyDate).ToList();
                ViewBag.comboboxEP = DA_Employee.Instance.GetAll().OrderBy(x => x.FullName).ToList();
           
                return View(DA_Receipt.Instance.GetById(Convert.ToInt32(id)));
            }

            return RedirectToAction("Index", "Receipt");
        }

        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");

            ViewBag.comboboxPT = DA_Party.Instance.GetAll().OrderByDescending(x => x.PartyDate).ToList();
            ViewBag.comboboxEP = DA_Employee.Instance.GetAll().OrderBy(x => x.FullName).ToList();
            return View();
        }
        #endregion

        #region method for ajax
        [HttpPost]
        public JsonResult GetReceiptDatatableIndex()
        {
            try
            {
                #region get para from view
                var valueDateFrom = Request.Form.GetValues("fromDate").FirstOrDefault();
                var valueDateTo = Request.Form.GetValues("toDate").FirstOrDefault();
                DateTime ValueDateTo = DateTime.Now;
                DateTime ValueDateFrom = DateTime.Now;
                DateTime.TryParseExact(valueDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDateFrom);             
                DateTime.TryParseExact(valueDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDateTo);

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

                List<object> data = DA_Receipt.Instance.getReceiptForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir, ValueDateFrom, ValueDateTo);
                recordsTotal = DA_Receipt.Instance.countAllReceiptFlowSearch(search.ToString());
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
                if (id.All(char.IsDigit))
                {
                    return Json(DA_Receipt.Instance.Delete(Convert.ToInt32(id)) > 0 ? 1 : 0);
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
        public JsonResult AddOrUpdateEntity(TBL_RECEIPT item, bool isEdit, string receiptDate)
        {
            if (!string.IsNullOrWhiteSpace(item.ReceiptNo) && ((isEdit && item.ReceiptID > 0) || !isEdit))
            {
                try
                {
                    item.ReceiptDate = new DateTime(Convert.ToInt32(receiptDate.Split('/')[2]), Convert.ToInt32(receiptDate.Split('/')[1]), Convert.ToInt32(receiptDate.Split('/')[0]));
                    int result = isEdit ? DA_Receipt.Instance.Update(item) : DA_Receipt.Instance.Insert(item);
                    return Json(result > 0 ? 1 : 0);
                }
                catch (Exception ex) { }
            }
            return Json(0);
        }
        [HttpPost]
        public JsonResult CheckReceiptNo(string receiptNo, int receiptID, bool isEdit)
        {
                try
                {
                    return Json(DA_Receipt.Instance.checkReceiptNo(receiptNo,receiptID,isEdit) );
                }
                catch (Exception ex) { }
            return Json(false);
        }


        #region delete option
        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var id in lsIdItem)
                    {
                        DA_Receipt.Instance.Delete(Convert.ToInt32(id));
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