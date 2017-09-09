using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Models;
using System.Data.SqlClient;
using System.Data;

namespace QUANLYTIEC.Controllers
{
    public class FastTimekeepingController : BaseController
    {
        #region method load 
        public ActionResult Index()
        {
            ViewBag.comboboxNV = DA_Employee.Instance.GetAll().OrderBy(n => n.FullName).ToList();
            return View();
        }
        #endregion

        #region method ajax

        [HttpPost]
        public JsonResult CalculatorSalaryShift(string dateFrom, string dateTo)
        {
            try
            {
                DateTime DateFrom = new DateTime(2000, 1, 1, Convert.ToInt32(dateFrom.Split(':')[0]), Convert.ToInt32(dateFrom.Split(':')[1]), 0);
                DateTime DateTo = new DateTime(2000, 1, 1, Convert.ToInt32(dateTo.Split(':')[0]), Convert.ToInt32(dateTo.Split(':')[1]), 0);
                Database getData = new Database();
                getData.fn_GetData_Pro("pr_CalPayRoll", new SqlParameter("@FromDate", DateFrom), new SqlParameter("@ToDate", DateTo));
                DataTable data = getData.mn_Table;
                return Json(Convert.ToInt32(data.Rows[0]["Amount"]), JsonRequestBehavior.AllowGet );
            }
            catch (Exception ex) { return Json(0); }
        }
        [HttpPost]
        public JsonResult AddOrUpdateListEntity(List<TBL_FAST_PAYROLL> lsEntity, List<string> lsCheckIn, List<string> lsCheckOut)
        {
            try
            {
                var listEnumerator = lsEntity.GetEnumerator();
                for (int i = 0; listEnumerator.MoveNext() == true; i++)
                {
                    DateTime now = DateTime.Now;
                    TBL_FAST_PAYROLL currentItem = listEnumerator.Current;
                    string checkInCurrent = lsCheckIn.ElementAt(i);
                    string checkOutCurrent = lsCheckOut.ElementAt(i);
                    currentItem.CheckIn = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(checkInCurrent.Split(':')[0]), Convert.ToInt32(checkInCurrent.Split(':')[1]), 0);
                    currentItem.CheckOut = new DateTime(now.Year, now.Month, now.Day, Convert.ToInt32(checkOutCurrent.Split(':')[0]), Convert.ToInt32(checkOutCurrent.Split(':')[1]), 0);
                }
                DA_FastPayroll.Instance.Insert(lsEntity);
                return Json(1);

            }catch (Exception ex){return Json(0);}
        }
        #endregion
    }
}