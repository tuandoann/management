using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace QUANLYTIEC.Controllers
{
    public class PrintDetailMaterialController : BaseController
    {
        // GET: ReportQuantitativeMenu
        #region biến
        static DataTable dtToExcel;
        #endregion

        #region method

        public ActionResult GetViewReportQuantitative(int id)
        {

            if (!CheckPermission())
                return RedirectToAction("Index", "Login");

            ViewBag.PartyType = DA_PARTY_PRODUCT_MATERIAL.Instance.GetPartyType(id);
            return View(DA_Party.Instance.GetById(id));
        }

        [HttpPost]
        public JsonResult ExportEXcel(int partyID)
        {
            string refer = "";
            return Json((!exportExcel(partyID, ref refer, DA_PARTY_PRODUCT_MATERIAL.Instance.GetPartyType(partyID), DA_Party.Instance.GetById(partyID))) ? "0!" + refer : "1!/Dinh luong tiec.xlsx");
        }

        public bool exportExcel(int partyID, ref string refer, SYS_PARTY_TYPE objSYS_PARTY_TYPE, TBL_PARTY objTBL_PARTY)
        {
            var a = objSYS_PARTY_TYPE == null ? "" : objSYS_PARTY_TYPE.PartyTypeName;     
            List<dynamic> lstDA_PARTY_PRODUCT_MATERIAL = DA_PARTY_PRODUCT_MATERIAL.Instance.GetViewReportForDatatablePagging(0, int.MaxValue, "MaterialName", "", Convert.ToInt32(partyID));

            using (ExcelPackage pck = new ExcelPackage())
            {

                try
                {
                    //tạo dữ liệu khi các biến toàn cục không có dữ liệu

                    var wsList = pck.Workbook.Worksheets.Add("Báo cáo");
                    ////int nCot = dtToExcel.Columns.Count;
                    ////int nDong = dtToExcel.Rows.Count;

                    //////định dạng chung cho cả sheet
                    //wsList.Cells[1, 1, nDong + 3, 4].Style.Font.Name = "Tahoma";
                    //wsList.Cells[1, 1, nDong + 3, 4].Style.Font.Size = 11;

                    wsList.Cells[4, 1, 4, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, 4, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, 4, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, 4, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    #region phần cứng
                    //đỗ dữ liệu
                    wsList.Cells["A1"].LoadFromText("ĐỊNH LƯỢNG THEO TIỆC NGÀY " + objTBL_PARTY.PartyDate.ToString("dd/MM/yyyy"));//cong ngay
                    wsList.Cells[2, 1].LoadFromText("Ông/ bà: ");
                    wsList.Cells[2, 2].LoadFromText(objTBL_PARTY.CustomerName.ToString());
                    wsList.Cells[2, 3].LoadFromText("Tiệc: " + a);
                    wsList.Cells[2, 4].LoadFromText("Giờ: " + objTBL_PARTY.PartyDate.ToString("hh'h'mm"));
                    wsList.Cells[3, 1].LoadFromText("Địa chỉ: ");
                    wsList.Cells[3, 2].LoadFromText(objTBL_PARTY.PartyAddress);
                    //wsList.Cells[2, 2].LoadFromText("gan value");
                    //wsList.Cells[2, 2].LoadFromText("gan value");

                    //wsList.Cells["B2"].LoadFromText(date);
                    // wsList.Cells[nDong + 3, 1].LoadFromText("Tổng cộng");
                    //wsList.Cells[nDong + 3, 4].LoadFromText(SUM.ToString());
                    //style
                    wsList.Cells[1, 1, 1, 8].Merge = true;
                    //   wsList.Cells[3, 1, 3, 7].Merge = true;
                    wsList.Cells[1, 1, 1, 7].Style.Font.Bold = true;
                    wsList.Cells[4, 1, 4, 7].Style.Font.Bold = true;
                    wsList.Cells[2, 1, 3, 1].Style.Font.Bold = true;
                    //wsList.Cells[1, 1, 1, nCot - 1].Style.Font.Size = 14;
                    wsList.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[4, 1, 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // wsList.Cells[2, 2, 2, 4].Merge = true;
                    // wsList.Cells[2, 2, 2, nCot - 1].Style.Font.Bold = true;
                    //// wsList.Cells["A2"].Style.Font.Size = 14;
                    // wsList.Cells[2, 2, 2, nCot - 1].Style.Font.Size = 12;
                    // wsList.Cells[2, 1, 2, nCot - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //wsList.Cells[nDong + 3, 1, nDong + 3, 4].Style.Font.Bold = true;
                    //wsList.Cells[nDong + 3, 1, nDong + 3, 3].Merge = true;
                    //wsList.Cells[nDong + 3, 1, nDong + 3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion

                    #region dữ liệu
                    //tiêu đề
                    wsList.Cells[4, 1].LoadFromText("STT");
                    wsList.Cells[4, 2].LoadFromText("NGUYÊN LIỆU");
                    wsList.Cells[4, 3].LoadFromText("SỐ LƯỢNG/TRỌNG LƯỢNG");
                    wsList.Cells[4, 4].LoadFromText("ĐVT");
                    wsList.Cells[4, 5].LoadFromText("ĐƠN GIÁ");
                    wsList.Cells[4, 6].LoadFromText("NHÀ CUNG CẤP");
                    wsList.Cells[4, 7].LoadFromText("GIAO HÀNG TRƯỚC");

                    //Hien thi du lieu
                    try
                    {
                        ////for (int i = 0; i < nCot; i++)
                        // {
                        for (int j = 0; j < lstDA_PARTY_PRODUCT_MATERIAL.Count(); j++)
                        {

                            wsList.Cells[j + 5, 1].LoadFromText((j + 1).ToString());
                            wsList.Cells[j + 5, 2].LoadFromText(lstDA_PARTY_PRODUCT_MATERIAL[j].MaterialName == null ? "" : lstDA_PARTY_PRODUCT_MATERIAL[j].MaterialName.ToString());

                            decimal quantity = lstDA_PARTY_PRODUCT_MATERIAL[j].Quantity;
                            // wsList.Cells[j + 5, 3].LoadFromText(string.Format("{#,##0.####}", quantity));
                            wsList.Cells[j + 5, 3].Value = quantity;
                            wsList.Cells[j + 5, 3].Style.Numberformat.Format = "#,##0.0000";

                            decimal unitPrice = lstDA_PARTY_PRODUCT_MATERIAL[j].UnitPrice ?? "";
                            wsList.Cells[j + 5, 5].Value = unitPrice;
                            wsList.Cells[j + 5, 5].Style.Numberformat.Format = "#,##0";

                            wsList.Cells[j + 5, 4].LoadFromText(lstDA_PARTY_PRODUCT_MATERIAL[j].UOMName == null ? "" : lstDA_PARTY_PRODUCT_MATERIAL[j].UOMName.ToString());
                            wsList.Cells[j + 5, 6].LoadFromText(lstDA_PARTY_PRODUCT_MATERIAL[j].VendorName == null ? "" : lstDA_PARTY_PRODUCT_MATERIAL[j].VendorName.ToString());

                            // bool isDelivery = lstDA_PARTY_PRODUCT_MATERIAL[j].UnitPrice;
                            wsList.Cells[j + 5, 7].LoadFromText(lstDA_PARTY_PRODUCT_MATERIAL[j].IsDelivery == true ? "Có" : "Không");


                            wsList.Cells[1, 1, j + 5, 7].Style.Font.Name = "Times New Roman";
                            wsList.Cells[1, 1, j + 3, 7].Style.Font.Size = 11;
                            wsList.Cells[j + 5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            wsList.Cells[j + 5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            wsList.Cells[j + 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            wsList.Cells[j + 5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            wsList.Cells[j + 5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;                          
                            wsList.Cells[j + 5, 7].Merge = true;
                            //Khung Viền
                            wsList.Cells[j + 5, 1, j + 5, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 5, 1, j + 5, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 5, 1, j + 5, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 5, 1, j + 5, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            //wsList.Cells[3, 1, nDong + 3, 4]
                            //wsList.Cells[3, 1, nDong + 3, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            //wsList.Cells[3, 1, nDong + 3, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            //wsList.Cells[3, 1, nDong + 3, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        }
                        //}
                    }
                    catch (Exception ex)
                    {
                        string e = ex.Message;
                    }

                    #endregion
                    //định dạng wrap text

                    //định dạng width cho sheet
                    wsList.Column(1).Width = 10;
                    wsList.Column(2).Width = 20;
                    wsList.Column(3).Width = 30;
                    wsList.Column(4).Width = 10;
                    wsList.Column(5).Width = 15;
                    wsList.Column(6).Width = 20;
                    wsList.Column(7).Width = 25;

                    string fileName = (Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)) + "/Dinh luong tiec.xlsx").Remove(0, 6);
                    Stream stream = System.IO.File.Create(fileName);
                    pck.SaveAs(stream);
                    stream.Close();
                    pck.Dispose();
                    return true;
                }

                catch (Exception ex)
                {
                    refer = ex.Message;
                    return false;
                }
            }
        }

        //Lay dữ liệu cho bảng
        [HttpPost]
        public JsonResult GetViewReportDatatableIndex()
        {
            try
            {


                #region get para from view
                //jQuery DataTables Param
                var id = Request.Form.GetValues("id").FirstOrDefault();
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                //Find paging info
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault());
                //Find order columns info
                var sortColumn = Request.Form.GetValues("columns[" + (orderColumn == 0 ? 1 : orderColumn) + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                List<object> data = DA_PARTY_PRODUCT_MATERIAL.Instance.GetViewReportForDatatablePagging(skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir, Convert.ToInt32(id));
                return Json(new { draw = draw, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        #endregion
    }
}