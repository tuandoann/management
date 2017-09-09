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
    public class ReportProductListController : BaseController
    {
        //
        // GET: /ReportProductList/
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        [HttpPost]
        public JsonResult GetEntityForDatatable()
        {
            try
            {
                Database getData = new Database();
                getData.fn_GetData_Pro("pr_ReportProductList");
                DataTable data = getData.mn_Table;
                var result = data.AsEnumerable().Select(m => new
                {
                    GroupName2 = m.Field<string>("GroupName2") ?? "",
                    GroupName1 = m.Field<string>("GroupName1") ?? "",
                    ProductName = m.Field<string>("ProductName") ?? "",
                    Notes = m.Field<string>("Notes") ?? "",
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