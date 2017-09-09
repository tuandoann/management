using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;

namespace QUANLYTIEC.Controllers
{
    public class ReportPartyListController : BaseController
    {
        //
        // GET: /ReportPartyList/
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
                #region get parameter for method post
                DateTime dateFrom = DateTime.Now;
                DateTime.TryParseExact(Request.Form.GetValues("dateFrom").FirstOrDefault().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom);
                DateTime dateTo = DateTime.Now;
                DateTime.TryParseExact(Request.Form.GetValues("dateTo").FirstOrDefault().ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTo);
                #endregion
                Database getData = new Database();
                getData.fn_GetData_Pro("pr_ReportPartyList", new SqlParameter("@FromDate", dateFrom), new SqlParameter("@ToDate", dateTo));
                DataTable data = getData.mn_Table;
                var result = data.AsEnumerable().Select(m => new
                {
                    PartyDate = m.Field<DateTime>("PartyDate").ToString("dd/MM/yyyy"),
                    CustomerName = m.Field<String>("CustomerName") ?? "",
                    PartyType = m.Field<String>("PartyType") ?? "",
                    PartyAddress = m.Field<String>("PartyAddress") ?? "",
                    NumberTablePlan = m.Field<int>("NumberTablePlan"),
                    NumberTableException = m.Field<int>("NumberTableException"),
                    NumberTableVegetarian = m.Field<int>("NumberTableVegetarian"),
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