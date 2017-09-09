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
    public class MaterialController : BaseController
    {
        #region method is used for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_MATERIAL).Name, "index");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.comBoBoxDVT = DA_UOM.Instance.GetAll().ToList();
                ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_MATERIAL).Name, "edit");
                return View(DA_Material.Instance.GetById(Convert.ToInt32(id)));
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_MATERIAL).Name, "index");
            return RedirectToAction("Index", "Material");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_MATERIAL).Name, "create");
            ViewBag.comBoBoxDVT = DA_UOM.Instance.GetAll().ToList();
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMaterialDatatableIndex()
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
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_Material.Instance.getMaterialForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_Material.Instance.countAllMaterialFlowSearch(search.ToString());
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
                string id = (Request.Form.GetValues("id").FirstOrDefault() ?? "").ToString();
                if (id.All(char.IsDigit))
                {
                    if(!DA_Material.Instance.UsingMaterial(Convert.ToInt32(id)))
                    {
                        
                        return Json(DA_Material.Instance.Delete(Convert.ToInt32(id)) > 0 ? 1 : 0);
                    }
                    else
                    {
                        return Json("Dữ liệu đang được sử dụng");
                    }
                }
                return Json(0);

            }
            catch (Exception ex){return Json(0); }
        }

        public JsonResult GetUOMID()
        {
            try
            {
                //jQuery DataTables Param
                string id = (Request.Form.GetValues("id").FirstOrDefault() == null
                    ? ""
                    : Request.Form.GetValues("id").FirstOrDefault().ToString());
                if (id.All(Char.IsDigit))
                {
                    TBL_MATERIAL item = DA_Material.Instance.GetById(Convert.ToInt32(id));
                    return Json(item == null ? 1 : item.UOMID);
                }
            }
            catch (Exception ex) { }
            return Json(1);
        }
        [HttpPost]
        public JsonResult AddOrUpdateEntity(TBL_MATERIAL item, bool isEdit)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(item.MaterialName) && ((isEdit && item.MaterialID > 0) || !isEdit))
                {
                    int result = isEdit ? DA_Material.Instance.Update(item) : DA_Material.Instance.Insert(item);
                    return Json(result > 0 ? 1 : 0);
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }


        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (int id in lsIdItem)
                    {
                        if (!DA_Material.Instance.UsingMaterial(Convert.ToInt32(id)))
                       
                            DA_Material.Instance.Delete(Convert.ToInt32(id));                   
                        else
                        {
                            TBL_MATERIAL item = DA_Material.Instance.GetById(id);
                            throw new Exception("Nguyên liệu " + item.MaterialName + " đang sử dụng");
                        }
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