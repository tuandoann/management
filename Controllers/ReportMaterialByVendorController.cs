using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.Data.SqlClient;
using System.Globalization;

namespace QUANLYTIEC.Controllers
{
    public class ReportMaterialByVendorController : BaseController
    {
        //
        // GET: /ReportMaterialByVendor/
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.Vendor = DA_Vendor.Instance.GetAll().ToList();
            return View();
        }
        [HttpPost]
        public JsonResult GetEntityForDatatable()
        {
            try
            {
                #region get parameter for method post
                DateTime dateFrom = DateTime.Now;
                DateTime.TryParseExact(Request.Form.GetValues("dateFrom").FirstOrDefault().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom);
                DateTime dateTo = DateTime.Now;
                DateTime.TryParseExact(Request.Form.GetValues("dateTo").FirstOrDefault().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTo);
                int vendor = Convert.ToInt32(Request.Form.GetValues("vendor").FirstOrDefault() ?? "0");
                #endregion
                Database getData = new Database();
                getData.fn_GetData_Pro("pr_ReportMaterialByVendor", new SqlParameter("@FromDate", dateFrom), new SqlParameter("@ToDate", dateTo), new SqlParameter("@VendorID", vendor));
                DataTable data = getData.mn_Table;
                var result = data.AsEnumerable().Select(m => new
                {
                    MaterialName = m.Field<string>("MaterialName") ?? "",
                    VendorName = m.Field<string>("VendorName") ?? "",
                    HomePhone = m.Field<string>("HomePhone") ?? "",
                    PhoneNumber = m.Field<string>("PhoneNumber") ?? "",
                    Address = m.Field<string>("Address") ?? "",
                });
                return Json(new { data = result.ToList<object>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
	}
}