using OfficeOpenXml;
using OfficeOpenXml.Style;
using QUANLYTIEC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models.BUS;
using System.Linq.Dynamic;
using System.Web.DynamicData;

namespace QUANLYTIEC.Controllers
{
    public class ReportController : BaseController
    {

        #region ReportAttendance
        public ActionResult ReportAttendance()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.DateTo = DateTime.Now;
            ViewBag.DateFrom = DateTime.Now;
            ViewBag.Employee = DA_Employee.Instance.GetAll().OrderBy("FullName asc").ToList();
            ViewBag.EmployeeId = 0;
            return View();
        }
        [HttpPost]
        public ActionResult ReportAttendance(string dateFrom, string dateTo, string employee)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            #region check data parameter
            DateTime DateFrom = new DateTime();
            DateTime DateTo = new DateTime();
            DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateFrom);
            DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTo);
            int employeeId = employee.All(Char.IsDigit) ? Convert.ToInt32(employee) : 0;
            #endregion
            Database getData = new Database();
            getData.fn_GetData_Pro("pr_ReportAttendance",
                new SqlParameter("@FromDate", DateFrom),
                new SqlParameter("@ToDate", DateTo),
                new SqlParameter("@EmployeeID", employeeId));
            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;
            ViewBag.dataTable = getData.mn_Table;
            var i = ViewBag.dataTable;
            ViewBag.EmployeeId = employeeId;
            ViewBag.Employee = DA_Employee.Instance.GetAll().OrderBy("FullName asc").ToList();
            return View();
        }
        [HttpPost]
        public JsonResult ExportReportAttendance()
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                try
                {
                    var wsList = pck.Workbook.Worksheets.Add("Chấm công nhân viên");
                    int colLast = 5;
                    #region lấy dữ liệu
                    var dateFrom = Request.Form.GetValues("dateFrom").FirstOrDefault();
                    var dateTo = Request.Form.GetValues("dateTo").FirstOrDefault();
                    var employeeId = Request.Form.GetValues("employeeId").FirstOrDefault().ToString();
                    DateTime DateFrom = DateTime.Now;
                    DateTime DateTo = DateTime.Now;
                    DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateFrom);
                    DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTo);
                    int EmployeeId = Convert.ToInt32(employeeId);
                    Database getData = new Database();
                    getData.fn_GetData_Pro("pr_ReportAttendance", new SqlParameter("@FromDate", DateFrom), new SqlParameter("@ToDate", DateTo), new SqlParameter("@EmployeeID", EmployeeId));
                    DataTable data = getData.mn_Table;
                    #endregion

                    #region đỗ dữ liệu vào excel
                    //tiêu đề
                    wsList.Cells["A1"].LoadFromText("Chấm công nhân viên");
                    wsList.Cells["A2"].LoadFromText("Từ ngày: " + dateFrom.ToString() + " đến ngày: " + dateTo.ToString());
                    wsList.Cells["A3"].LoadFromText("Nhân viên: " + ((EmployeeId == 0) ? "Tất cả " : DA_Employee.Instance.GetById(EmployeeId).FullName));
                    //table
                    wsList.Cells["A4"].LoadFromText("Ngày");
                    wsList.Cells["B4"].LoadFromText("Giờ đến");
                    wsList.Cells["C4"].LoadFromText("Giờ đi");
                    wsList.Cells["D4"].LoadFromText("Tiền lương");
                    wsList.Cells["E4"].LoadFromText("Đơn vị");
                    int index = 5;
                    long sum = 0;
                    string nameEmployeeBefore = "";
                    long sumItem = 0;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        string nameEmployeeCurrent = data.Rows[i]["FullName"].ToString();
                        sum += Convert.ToInt64(data.Rows[i]["TotalAmount"]);
                        if (nameEmployeeBefore != nameEmployeeCurrent)
                        {
                            nameEmployeeBefore = nameEmployeeCurrent;
                            if (i != 0)
                            {
                                wsList.Cells[index + i, 1].LoadFromText("Cộng");
                                wsList.Cells[index + i, 4].Value = sumItem;
                                wsList.Cells[index + i, 4].Style.Numberformat.Format = "#,##0";
                                wsList.Cells[index + i, 1, index + i, 3].Merge = true;
                                wsList.Cells[index + i, 1, index + i, colLast].Style.Font.Bold = true;
                                wsList.Cells[index + i, 1, index + i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                wsList.Cells[index + i, 1, index + i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                sumItem = 0;
                                index += 1;
                            }
                            wsList.Cells[index + i, 1].LoadFromText("Tên nhân viên: " + data.Rows[i]["FullName"].ToString() + "        Vị trí: " + data.Rows[i]["Position"].ToString());
                            wsList.Cells[index + i, 1, index + i, colLast].Style.Font.Bold = true;
                            wsList.Cells[index + i, 1, index + i, colLast].Merge = true;
                            index += 1;
                        }
                        sumItem += Convert.ToInt64(data.Rows[i]["TotalAmount"]);

                        wsList.Cells[index + i, 1].Value = Convert.ToDateTime(data.Rows[i]["Date"]);
                        wsList.Cells[index + i, 1].Style.Numberformat.Format = "dd/MM/yyyy";
                        wsList.Cells[index + i, 2].Value = Convert.ToDateTime(data.Rows[i]["CheckIn"]);
                        wsList.Cells[index + i, 2].Style.Numberformat.Format = "HH:mm";
                        wsList.Cells[index + i, 3].Value = Convert.ToDateTime(data.Rows[i]["CheckOut"]);
                        wsList.Cells[index + i, 3].Style.Numberformat.Format = "HH:mm";
                        wsList.Cells[index + i, 4].Value = Convert.ToDecimal(data.Rows[i]["TotalAmount"]);
                        wsList.Cells[index + i, 4].Style.Numberformat.Format = "#,##0";
                        wsList.Cells[index + i, 5].LoadFromText(data.Rows[i]["UOM"].ToString());

                        if (i == (data.Rows.Count - 1))
                        {
                            index += 1;
                            wsList.Cells[index + i, 1].LoadFromText("Cộng");
                            wsList.Cells[index + i, 4].Value = sumItem;
                            wsList.Cells[index + i, 4].Style.Numberformat.Format = "#,##0";
                            wsList.Cells[index + i, 1, index + i, 3].Merge = true;
                            wsList.Cells[index + i, 1, index + i, colLast].Style.Font.Bold = true;
                            wsList.Cells[index + i, 1, index + i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            wsList.Cells[index + i, 1, index + i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            index += 1;
                            wsList.Cells[index + i, 1].LoadFromText("Tổng cộng");
                            wsList.Cells[index + i, 4].Value = sum;
                            wsList.Cells[index + i, 4].Style.Numberformat.Format = "#,##0";
                            wsList.Cells[index + i, 1, index + i, 3].Merge = true;
                            wsList.Cells[index + i, 1, index + i, colLast].Style.Font.Bold = true;
                            wsList.Cells[index + i, 1, index + i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            wsList.Cells[index + i, 1, index + i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sumItem = 0;
                        }
                    }
                    #endregion

                    #region style
                    int rowLast = index + data.Rows.Count - 1;
                    wsList.Cells[1, 1, rowLast, colLast].Style.Font.Name = "Times New Roman";
                    wsList.Cells[1, 1, rowLast, colLast].Style.Font.Size = 11;
                    wsList.Cells[1, 1, 1, colLast].Merge = true;
                    wsList.Cells[2, 1, 2, colLast].Merge = true;
                    wsList.Cells[3, 1, 3, colLast].Merge = true;
                    wsList.Cells[1, 1, 1, colLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[2, 1, 2, colLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[3, 1, 3, colLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[1, 1, 4, colLast].Style.Font.Bold = true;
                    wsList.Cells[4, 1, rowLast, colLast].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, colLast].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, colLast].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, rowLast, colLast].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[4, 1, 4, colLast].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[4, 1, 4, colLast].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    if (data.Rows.Count > 0)
                    {
                        wsList.Cells[5, 1, rowLast, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        wsList.Cells[5, 1, rowLast, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsList.Cells[5, 2, rowLast, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        wsList.Cells[5, 2, rowLast, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsList.Cells[5, 3, rowLast, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        wsList.Cells[5, 3, rowLast, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsList.Cells[5, 4, rowLast, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsList.Cells[5, 4, rowLast, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsList.Cells[5, 5, rowLast, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        wsList.Cells[5, 5, rowLast, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    wsList.Column(1).Width = 12;
                    wsList.Column(2).Width = 10;
                    wsList.Column(3).Width = 10;
                    wsList.Column(4).Width = 15;
                    wsList.Column(5).Width = 10;
                    #endregion

                    string path = Server.MapPath("/Cham_cong_nhan_vien.xlsx");
                    Stream stream = System.IO.File.Create(path);
                    pck.SaveAs(stream);
                    stream.Close();
                    return Json("1!/Cham_cong_nhan_vien.xlsx");
                }
                catch (Exception ex)
                {
                    return Json("0!" + ex.Message);
                }
            }
        }
        #endregion

        #region ReportSalary
        public ActionResult ReportSalary()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.DateTo = DateTime.Now;
            ViewBag.DateFrom = DateTime.Now;
            ViewBag.EmployeeId = 0;
            ViewBag.Employee = DA_Employee.Instance.GetAll().OrderBy("FullName asc").ToList();
            return View();
        }
        [HttpPost]
        public ActionResult ReportSalary(string dateFrom, string dateTo, string employee)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            #region check data parameter
            DateTime DateFrom = DateTime.Now;
            DateTime DateTo = DateTime.Now;
            DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateFrom);
            DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTo);

            int employeeId = employee.All(Char.IsDigit) ? Convert.ToInt32(employee) : 0;
            #endregion
            Database getData = new Database();
            getData.fn_GetData_Pro("pr_ReportPayroll",
                new SqlParameter("@FromDate", DateFrom),
                new SqlParameter("@ToDtate", DateTo),
                new SqlParameter("@EmployeeID", employeeId));
            ViewBag.DateTo = DateTo;
            ViewBag.DateFrom = DateFrom;
            ViewBag.EmployeeId = employeeId;
            ViewBag.dataTable = getData.mn_Table;
            TempData["salary"] = ViewBag.dataTable;
            TempData.Keep("salary");
            ViewBag.Employee = DA_Employee.Instance.GetAll().OrderBy("FullName asc").ToList();
            return View();
        }
        [HttpPost]
        public JsonResult ExportEXcel(string timeFrom, string timeTo, string selectEmployee)
        {
            DateTime DateFrom = DateTime.Now;
            DateTime DateTo = DateTime.Now;
            DateTime.TryParseExact(timeFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateFrom);
            DateTime.TryParseExact(timeTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTo);
            string refer = "";
            return Json((!ExportExcelSalary(ref refer, DateFrom, DateTo, selectEmployee)) ? "0!" + refer : "1!/Thong_ke_luong.xlsx");

        }
        public bool ExportExcelSalary(ref string refer, DateTime timeFrom, DateTime timeTo, string selectEmployee)
        {
            dynamic data = TempData["salary"];
            using (ExcelPackage pck = new ExcelPackage())
            {

                try
                {
                    //t?o d? li?u khi các bi?n toàn c?c không có d? li?u
                    var wsList = pck.Workbook.Worksheets.Add("Báo cáo");

                    //////d?nh d?ng chung cho c? sheet                
                    wsList.Cells[3, 1, 3, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[3, 1, 3, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[3, 1, 3, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsList.Cells[3, 1, 3, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    #region ph?n 
                    //d? d? li?u
                    wsList.Cells["A1"].LoadFromText("THỐNG KÊ LUƠNG");//cong ngay
                    wsList.Cells["A1"].Style.Font.Bold = true;
                    int rowLast = data.Rows.Count;

                    wsList.Cells[rowLast + 4, 4].LoadFromText("Tổng cộng: ");
                    //wsList.Cells[rowLast + 4, 4, rowLast + 4, 5].Merge = true;
                    wsList.Cells[rowLast + 4, 4, rowLast + 4, 5].Style.Font.Bold = true;
                    wsList.Cells[rowLast + 4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsList.Cells[rowLast + 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    wsList.Cells[2, 1].LoadFromText("Từ ngày: " + timeFrom.ToString("dd/MM/yyyy"));
                    wsList.Cells[2, 2].LoadFromText("Ðến ngày: " + timeTo.ToString("dd/MM/yyyy"));
                    wsList.Cells[2, 3].LoadFromText("Nhân viên: " + (Convert.ToUInt32(selectEmployee) == 0 ? "Tất cả" : DA_Employee.Instance.GetById(Convert.ToInt32(selectEmployee)).FullName));


                    #region d? d? li?u vào excel
                    //tiêu d?
                    wsList.Cells["A1"].LoadFromText("THỐNG KÊ LUƠNG ");

                    //table
                    wsList.Cells["A3"].LoadFromText("STT");
                    wsList.Cells["B3"].LoadFromText("TÊN NHÂN VIÊN");
                    wsList.Cells["C3"].LoadFromText("VỊ TRÍ");
                    wsList.Cells["D3"].LoadFromText("TỔNG LƯƠNG");
                    wsList.Cells["E3"].LoadFromText("ÐƠN VỊ");


                    #endregion


                    wsList.Cells[1, 1, 1, 5].Merge = true;
                    wsList.Cells[3, 1, 3, 5].Style.Font.Bold = true;
                    // wsList.Cells[2, 1, 2, 3].Merge = true;
                    //wsList.Cells[2, 1, 3, 1].Style.Font.Bold = true;
                    //wsList.Cells[1, 1, 1, nCot - 1].Style.Font.Size = 14;
                    wsList.Cells[1, 1, 1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsList.Cells[3, 1, 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion

                    //Hien thi du lieu
                    try
                    {
                        long sum = 0;

                        for (int j = 0; j < data.Rows.Count; j++)
                        {

                            wsList.Cells[4 + j, 1].LoadFromText((j + 1).ToString());
                            wsList.Cells[4 + j, 2].LoadFromText(data.Rows[j]["FullName"].ToString());
                            wsList.Cells[4 + j, 3].LoadFromText(data.Rows[j]["Position"].ToString());
                            //tong luong                      
                            decimal totalAmount = data.Rows[j]["TotalAmount"];
                            wsList.Cells[4 + j, 4].Value = totalAmount;
                            wsList.Cells[4 + j, 4].Style.Numberformat.Format = "#,##0";
                            //cong tong luong
                            sum += Convert.ToInt64(data.Rows[j]["TotalAmount"]);
                            wsList.Cells[rowLast + 4, 5].Value = sum;
                            wsList.Cells[rowLast + 4, 5].Style.Numberformat.Format = "#,##0";
                            //don vi tinh
                            wsList.Cells[4 + j, 5].LoadFromText(data.Rows[j]["UOM"].ToString());
                            //style
                            wsList.Cells[1, 1, j + 5, 5].Style.Font.Name = "Times New Roman";
                            wsList.Cells[1, 1, j + 3, 5].Style.Font.Size = 11;
                            wsList.Cells[j + 4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            wsList.Cells[j + 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            wsList.Cells[4 + j, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            wsList.Cells[4 + j, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            wsList.Cells[4 + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            wsList.Cells[j + 4, 1, j + 4, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 4, 1, j + 4, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 4, 1, j + 4, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            wsList.Cells[j + 4, 1, j + 4, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        }

                    }
                    catch (Exception ex)
                    {
                        string e = ex.Message;
                    }


                    //d?nh d?ng wrap text

                    //d?nh d?ng width cho sheet
                    wsList.Column(1).Width = 20;
                    wsList.Column(2).Width = 30;
                    wsList.Column(3).Width = 20;
                    wsList.Column(4).Width = 23;
                    wsList.Column(5).Width = 15;


                    string fileName = (Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)) + "/Thong_ke_luong.xlsx").Remove(0, 6);
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

        #endregion

    }
}