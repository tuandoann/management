using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using System.Transactions;
using System.Globalization;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Style;

namespace QUANLYTIEC.Controllers
{
    public class PartyController : BaseController
    {
        #region method is used for load page
        public ActionResult PartyScheduler()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }

        public ActionResult ReportMaterialParty(string date)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");

            DateTime ValueDate = DateTime.Now;
            if(DateTime.TryParseExact(date, "dd/mm/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDate))
            {
                Database getData = new Database();
                SqlParameter parameter = new SqlParameter("@Date", SqlDbType.DateTime)
                {
                    Value = ValueDate
                };
                getData.fn_GetData_Pro("pr_Print_PRODUCT_MATERIAL_By_Date", parameter);

                ViewBag.dataTable = getData.mn_Table;
                ViewBag.Title = ValueDate.ToString("dd/mm/yyyy");
                return View();
            }
            return RedirectToAction("PartyScheduler", "Party");


        }


        public ActionResult ViewProductMaterial(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                int Id = Convert.ToInt32(id);
                var item = DA_Party.Instance.GetById(Id);
                if (item != null)
                {
                    ViewBag.ProductParty = DA_PartyProduct.Instance.getEntityBasePartyId(Id);
                    return View(item);
                }
            }
            return RedirectToAction("PartyScheduler", "Party");
        }

        public ActionResult MaterialParty()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            return View();
        }
        public ActionResult RegisterParty()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.ComboboxPartyType = DA_PartyType.Instance.GetAll();
            ViewBag.ComboboxFood = DA_Food.Instance.loadEntityForCombobox(true);
            ViewBag.ComboboxService = DA_Service.Instance.loadEntityForCombobox(true);
            return View();
        }
        public ActionResult EditParty(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                int Id = Convert.ToInt32(id);
                TBL_PARTY item = DA_Party.Instance.GetById(Id);
                if (item != null)
                {
                    ViewBag.ComboboxPartyType = DA_PartyType.Instance.GetAll();
                    ViewBag.ComboboxFood = DA_Food.Instance.loadEntityForCombobox(false);
                    ViewBag.ComboboxService = DA_Service.Instance.loadEntityForCombobox(false);
                    ViewBag.PartyFood = DA_PartyProduct.Instance.getEntityBasePartyId(Id);
                    ViewBag.PartyService = DA_PartyService.Instance.getEntityBasePartyId(Id);
                    return View(item);
                }
            }
            return RedirectToAction("PartyScheduler", "Party");
        }
        #endregion

        #region method is used for ajax

        #region ajax excute data
        /// <summary>
        /// ajax add or update entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOrUpdateEntity(TBL_PARTY objectParty, List<TBL_PARTY_PRODUCT> lsObjectMeal, List<TBL_PARTY_SERVICE> lsObjecService, bool isEdit, int[][] objectDate)
        {
            try
            {
                if (objectDate.Length >= 3)
                {
                    objectParty.BookingDate = new DateTime(objectDate[0][2], objectDate[0][1], objectDate[0][0]);
                    objectParty.PartyDate = new DateTime(objectDate[1][2], objectDate[1][1], objectDate[1][0], objectDate[1][3], objectDate[1][4], 0);
                    objectParty.NegativeDate = new DateTime(objectDate[2][2], objectDate[2][1], objectDate[2][0]);
                    objectParty.DepositDate = ((objectDate.Length > 3) ? new DateTime(objectDate[3][2], objectDate[3][1], objectDate[3][0]) : objectParty.DepositDate);
                    objectParty.UserCreate = Convert.ToInt32(Session["UserID"].ToString().All(Char.IsDigit) ? Session["UserID"] : 0);
                    if (isEdit)
                        return Json(DA_Party.Instance.ProcesseActionUpdateFormAllEntity(objectParty, lsObjectMeal, lsObjecService) ? 1 : 0);
                    else
                        return Json(DA_Party.Instance.ProcesseActionInsertFormAllEntity(objectParty, lsObjectMeal, lsObjecService) ? 1 : 0);
                }

            }
            catch (Exception ex) { }
            //}
            return Json(0);
        }

        /// <summary>
        /// ajax method delete entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteEntity(int id)
        {
            try
            {
                if (id != 0)
                    return Json(DA_Party.Instance.ProcesseActionDeleteFormAllEntity(id) ? 1 : 0);
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult AddOrUpdatePartyProductMaterial(List<TBL_PARTY_PRODUCT_MATERIAL> lsPartyProductMaterialInsert, List<TBL_PARTY_PRODUCT_MATERIAL> lsPartyProductMaterialUpdate)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    if (lsPartyProductMaterialInsert != null && lsPartyProductMaterialInsert.Count > 0)
                    {
                        DA_PartyProductMaterial.Instance.Insert(lsPartyProductMaterialInsert);
                    }
                    if (lsPartyProductMaterialUpdate != null && lsPartyProductMaterialUpdate.Count > 0)
                    {
                        DA_PartyProductMaterial.Instance.Update(lsPartyProductMaterialUpdate);
                    }
                    scope.Complete();
                    return Json(1);
                }
            }
            catch (Exception ex) { return Json(0); }
        }
        #endregion

        #region fill data
        /// <summary>
        /// get list entity for calendar
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPartyForFullCalendar()
        {
            try
            {
                var dateFrom = Request.Form.GetValues("start").FirstOrDefault();
                var dateTo = Request.Form.GetValues("end").FirstOrDefault();
                DateTime DateFrom, DateTo;
                if ((dateFrom != null && DateTime.TryParse((dateFrom ?? "").ToString(), out DateFrom)) 
                 && (dateTo != null && DateTime.TryParse((dateTo ?? "").ToString(), out DateTo)))
                {
                    return Json(DA_Party.Instance.GetEntityBasePartyDate(DateFrom, DateTo), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { }
            return Json(null);
        }
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPartyDatatableIndex()
        {
            try
            {
                #region get para from view
                var isSearchDate = Request.Form.GetValues("isSearchDate").FirstOrDefault();
                var valueDate = Request.Form.GetValues("valueDate").FirstOrDefault();
                DateTime ValueDate = DateTime.Now;
                Boolean IsSearchDate = Convert.ToBoolean(isSearchDate);
                if (IsSearchDate)
                    DateTime.TryParseExact(valueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDate);
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

                List<object> data = DA_Party.Instance.getPartyForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir, IsSearchDate, ValueDate);
                recordsTotal = DA_Party.Instance.countAllPartyFlowSearch(search.ToString());
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPartyDatatableReport()
        {
            try
            {
                var valueDate = Request.Form.GetValues("valueDate").FirstOrDefault();
                DateTime ValueDate = DateTime.Now;
                DateTime.TryParseExact(valueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDate);
                Database getData = new Database();
                SqlParameter parameter = new SqlParameter("@Date", SqlDbType.DateTime)
                {
                    Value = ValueDate
                };
                getData.fn_GetData_Pro("pr_Print_PRODUCT_MATERIAL_By_Date", parameter);
                DataTable data = getData.mn_Table;
                var result = data.AsEnumerable().Select(m => new
                {
                    MaterialName = m.Field<string>("MaterialName"),
                    Quantity = m.Field<Decimal>("Quantity"),
                    UOMName = m.Field<string>("UOMName"),
                    UnitPrice = m.Field<int>("UnitPrice"),
                    VendorName = m.Field<string>("VendorName"),
                    IsDelivery = m.Field<Boolean>("IsDelivery"),
                });
                return Json(new { data = result.ToList<object>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult GetUnitCostAndProfitAmountBaseProductID(int id)
        {
            try
            {
                if (id != 0)
                    return Json(DA_Party.Instance.GetUnitCostAndProfitAmountBaseProductID(id));
            }
            catch (Exception ex) { }
            return Json(0);
        }

        [HttpPost]
        public JsonResult MethodFillDataForDatatablesProductMaterial(int productId, int partyId)
        {
            try
            {
                if (productId != 0)
                    return Json(DA_Party.Instance.MethodFillDataForDatatablesProductMaterial(productId, partyId));
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult MethodFillDataWhenClickGetData(int productId, int partyId)
        {
            try
            {
                if (productId != 0)
                {
                    var item = DA_Party.Instance.MethodFillDataWhenClickGetData(productId, partyId);
                    return Json(item);
                }
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult GetIdAndCountProductInParty(int partyId)
        {
            try
            {
                if (partyId != 0)
                    return Json(DA_Party.Instance.GetIdAndCountProductInParty(partyId));
            }
            catch (Exception ex) { }
            return Json(0);
        }
        #endregion

        #region report
        [HttpPost]
        public JsonResult ExportMaterialParty()
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                try
                {
                    #region lấy dữ liệu
                    var valueDate = Request.Form.GetValues("valueDate").FirstOrDefault();
                    DateTime ValueDate = DateTime.Now;
                    DateTime.TryParseExact(valueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValueDate);
                    var wsList = pck.Workbook.Worksheets.Add("PrintDetailMaterial");
                    Database getData = new Database();
                    getData.fn_GetData_Pro("pr_Print_PRODUCT_MATERIAL_By_Date", new SqlParameter("@Date", ValueDate));
                    DataTable data = getData.mn_Table;
                    #endregion

                    #region đỗ dữ liệu vào excel
                    //tiêu đề
                    wsList.Cells["A1"].LoadFromText("ĐỊNH LƯỢNG THEO TIỆC NGÀY " + valueDate.ToString());
                    int rowLast = data.Rows.Count + 4;
                    //table
                    wsList.Cells["A4"].LoadFromText("STT");
                    wsList.Cells["B4"].LoadFromText("NGUYÊN LIỆU");
                    wsList.Cells["C4"].LoadFromText("SỐ LƯỢNG / TRỌNG LƯỢNG");
                    wsList.Cells["D4"].LoadFromText("ĐVT");
                    wsList.Cells["E4"].LoadFromText("ĐƠN GIÁ");
                    wsList.Cells["F4"].LoadFromText("NHÀ CUNG CẤP");
                    wsList.Cells["G4"].LoadFromText("GIAO HÀNG TRƯỚC");
                    int index = 5;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        wsList.Cells[index + i, 1].LoadFromText((i + 1).ToString());
                        wsList.Cells[index + i, 2].LoadFromText(data.Rows[i]["MaterialName"].ToString());
                        wsList.Cells[index + i, 3].Value = Convert.ToDecimal(data.Rows[i]["Quantity"]);
                        wsList.Cells[index + i, 3].Style.Numberformat.Format = "#,##0.0000";
                        wsList.Cells[index + i, 4].LoadFromText(data.Rows[i]["UOMName"].ToString());
                        wsList.Cells[index + i, 5].Value = Convert.ToInt32(data.Rows[i]["UnitPrice"]);
                        wsList.Cells[index + i, 5].Style.Numberformat.Format = "#,##0";
                        wsList.Cells[index + i, 6].LoadFromText(data.Rows[i]["VendorName"].ToString());
                        wsList.Cells[index + i, 7].LoadFromText(Convert.ToBoolean(data.Rows[i]["IsDelivery"]) ? "Có" : "Không");
                    }
                    #endregion

                    #region style
                    wsList.Cells[1, 1, rowLast, data.Columns.Count + 1].Style.Font.Name = "Times New Roman";
                    wsList.Cells[1, 1, rowLast, data.Columns.Count + 1].Style.Font.Size = 11;
                    wsList.Cells[1, 1, 1, data.Columns.Count + 1].Merge = true;
                    wsList.Cells[1, 1, 1, data.Columns.Count + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[1, 1, 4, data.Columns.Count + 1].Style.Font.Bold = true;
                    wsList.Cells[4, 1, rowLast, data.Columns.Count + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, data.Columns.Count + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, data.Columns.Count + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, data.Columns.Count + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, 4, data.Columns.Count + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[4, 1, 4, data.Columns.Count + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    wsList.Cells[5, 1, rowLast, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    wsList.Cells[5, 1, rowLast, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[5, 2, rowLast, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsList.Cells[5, 6, rowLast, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsList.Cells[5, 4, rowLast, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsList.Cells[5, 3, rowLast, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsList.Cells[5, 5, rowLast, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsList.Cells[5, 7, rowLast, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[5, 7, rowLast, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    wsList.Cells[5, 2, rowLast, 2].Style.WrapText = true;
                    wsList.Cells[5, 3, rowLast, 3].Style.WrapText = true;
                    wsList.Cells[5, 6, rowLast, 6].Style.WrapText = true;
                    wsList.Column(1).Width = 10;
                    wsList.Column(2).Width = 20;
                    wsList.Column(3).Width = 30;
                    wsList.Column(4).Width = 15;
                    wsList.Column(5).Width = 15;
                    wsList.Column(6).Width = 23;
                    wsList.Column(7).Width = 23;
                    #endregion

                    string path = Server.MapPath("/Dinh_luong_tiec_Theo_ngay.xlsx");
                    Stream stream = System.IO.File.Create(path);
                    pck.SaveAs(stream);
                    stream.Close();
                    return Json("1!/Dinh_luong_tiec_Theo_ngay.xlsx");
                }
                catch (Exception ex)
                {
                    return Json("0!" + ex.Message);
                }
            }
        }
        #endregion

        #endregion
    }
}