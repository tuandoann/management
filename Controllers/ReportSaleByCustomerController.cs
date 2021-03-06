﻿using System;
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
    public class ReportSaleByCustomerController : BaseController
    {
        //
        // GET: /ReportSaleByCustomer/
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
                getData.fn_GetData_Pro("pr_ReportSaleByCustomer", new SqlParameter("@FromDate", dateFrom), new SqlParameter("@ToDate", dateTo));
                DataTable data = getData.mn_Table;
                var result = data.AsEnumerable().Select(m => new
                {
                    CustomerName = m.Field<string>("CustomerName") ?? "",
                    SaleAmount = m.Field<Decimal>("SaleAmount"),
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