using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;
using System.Data.SqlClient;
using System.Transactions;

namespace QUANLYTIEC.Controllers
{
    public class FoodController : BaseController
    {

        #region method is used for load pagek
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            if (!Role(3))
            {
                return RedirectToAction("AccessDenied", "Errors");
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT).Name, "index");
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
            {
                return RedirectToAction("Index", "Login");
            }
            if (!Role(3))
            {
                return RedirectToAction("AccessDenied", "Errors");
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT).Name, "edit");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.comBoBoxNVL = DA_Material.Instance.GetAll().ToList();
                ViewBag.comBoBoxDVT = DA_UOM.Instance.GetAll().ToList();
                ViewBag.combobox = DA_GroupFood.Instance.getAllEntityNoChild();
                ViewBag.productMaterial = DA_Product_Material.Instance.getEntityBaseIdProduct(Convert.ToInt32(id));

                var item = DA_Food.Instance.GetById(Convert.ToInt32(id));
                return View(item);
            }
            return RedirectToAction("Index", "GroupFood");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT).Name, "create");
            ViewBag.comBoBoxNVL = DA_Material.Instance.GetAll().ToList();
            ViewBag.comBoBoxDVT = DA_UOM.Instance.GetAll().ToList();
            ViewBag.combobox = DA_GroupFood.Instance.getAllEntityNoChild();
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFoodDatatableIndex()
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
                var sortColumn = Request.Form.GetValues("columns[" + (orderColumn) + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_Food.Instance.getFoodForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_Food.Instance.countAllFoodFlowSearch(search.ToString());
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
                    if (!DA_Food.Instance.checkProduct(Convert.ToInt32(id)))
                        return Json(DA_Food.Instance.Delete(Convert.ToInt32(id)) > 0 ? 1 : 0);
                    else
                        return Json("dữ liệu đang được sử dụng");
                }
            }
            catch (Exception ex) { }
            return Json(0);
        }

        [HttpPost]
        public JsonResult AddOrUpdateEntity(TBL_PRODUCT product, List<TBL_PRODUCT_MATERIAL> productDetail, bool isEdit)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(product.ProductName))
                {
                    if (!isEdit)
                        return Json(MethodForInsertEntity(product, productDetail) ? 1 : 0);
                    else
                        return Json(MethodForUpdateEntity(product, productDetail) ? 1 : 0);
                }
            }
            catch (Exception ex) { }
            return Json(0);
        }
        [HttpPost]
        public JsonResult deleteMany(List<int> lsIdItem)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var id in lsIdItem)
                    {
                        if (!DA_Food.Instance.checkProduct(Convert.ToInt32(id)))
                            DA_Food.Instance.Delete(Convert.ToInt32(id));
                        else
                        {
                            TBL_PRODUCT item = DA_Food.Instance.GetById(Convert.ToInt32(id));
                            throw new Exception("Món ăn " + item.ProductName + " đang sử dụng");
                        }
                    }
                    scope.Complete();
                    return Json(1);
                }
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        #endregion

        #region method
        /// <summary>
        /// method use for insert entity
        /// </summary>
        /// <param name="product"></param>
        /// <param name="lsProductMaterial"></param>
        /// <returns></returns>
        private bool MethodForInsertEntity(TBL_PRODUCT product, List<TBL_PRODUCT_MATERIAL> lsProductMaterial)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    //insert entity
                    int resultExcuteRow = DA_Food.Instance.Insert(product);
                    bool resultExucte = resultExcuteRow == 0 ? false : true;
                    //insert list entity tbl_product_material
                    if (resultExucte && lsProductMaterial != null)
                    {
                        // setProductIdForListProductMaterial(ref lsProductMaterial, product.ProductID);
                        int result = DA_Product_Material.Instance.Insert(lsProductMaterial);
                        resultExucte = (result > 0 || (result == 0 && lsProductMaterial == null)) ? true : false;
                    }
                    scope.Complete();
                    return resultExucte;
                }
            }
            catch (Exception ex) { }
            return false;
        }
        /// <summary>
        /// method for update entity
        /// </summary>
        /// <param name="product"></param>
        /// <param name="lsProductMaterial"></param>
        /// <returns></returns>
        //private bool MethodForUpdateEntity(TBL_PRODUCT product, List<TBL_PRODUCT_MATERIAL> lsProductMaterial)
        //{
        //    try
        //    {
        //        using (var scope = new TransactionScope())
        //        {
        //            //update entity
        //            int resultExcuteRow = DA_Food.Instance.Update(product);
        //            bool resultExucte = resultExcuteRow == 0 ? false : true;
        //            if (resultExucte)
        //            {
        //              //  setProductIdForListProductMaterial(ref lsProductMaterial, product.ProductID);
        //                //get list object old of entity product
        //                List<TBL_PRODUCT_MATERIAL> lsProductMaterialOld = DA_Product_Material.Instance.getEntityBaseIdProduct(product.ProductID);
        //                lsProductMaterialOld = lsProductMaterialOld == null ? new List<TBL_PRODUCT_MATERIAL>() : lsProductMaterialOld;                // list productMaterial old
        //                List<TBL_PRODUCT_MATERIAL> lsProductMaterialNew = lsProductMaterial == null ? new List<TBL_PRODUCT_MATERIAL>() : lsProductMaterial;// productMaterial new
        //                if (lsProductMaterialNew.Count > 0 && lsProductMaterialOld.Count == 0)
        //                {
        //                    resultExucte = DA_Product_Material.Instance.Insert(lsProductMaterialNew) > 0 ? true : false;
        //                }
        //                if (lsProductMaterialNew.Count == 0 && lsProductMaterialOld.Count > 0)
        //                {
        //                    resultExucte = DA_Product_Material.Instance.deleteListEntity(lsProductMaterialOld);
        //                }
        //                if (lsProductMaterialNew.Count > 0 && lsProductMaterialOld.Count > 0)
        //                {
        //                    List<int> lsProductIdNew = lsProductMaterialNew.Select(n => n.ProductMaterialID).ToList();
        //                    List<int> lsProductIdOld = lsProductMaterialOld.Select(n => n.ProductMaterialID).ToList();
        //                    //array productId 
        //                    List<int> productIdDelete = lsProductIdOld.Except(lsProductIdNew).ToList();
        //                    List<int> productIdUpdate = lsProductIdNew.Where(n => lsProductIdOld.Any(p => p == n)).ToList();

        //                    //item exist array old not exist array new is array delete
        //                    List<TBL_PRODUCT_MATERIAL> lsDelete = lsProductMaterialOld.Where(n => productIdDelete.Any(p => p == n.ProductMaterialID)).ToList();
        //                    //item exist array new not exist array old is array delete
        //                    List<TBL_PRODUCT_MATERIAL> lsInsert = lsProductMaterialNew.Where(n => n.ProductMaterialID == 0).ToList();
        //                    // item exist array new and array old is array insert
        //                    List<TBL_PRODUCT_MATERIAL> lsUpdate = lsProductMaterialNew.Where(n => productIdUpdate.Any(p => p == n.ProductMaterialID)).ToList();
        //                    //excute
        //                    if (lsDelete != null && lsDelete.Count > 0)
        //                    {
        //                        resultExucte = DA_Product_Material.Instance.deleteListEntity(lsDelete);
        //                    }
        //                    if (lsInsert != null && lsInsert.Count > 0 && resultExucte)
        //                    {
        //                        resultExucte = DA_Product_Material.Instance.Insert(lsInsert) > 0 ? true : false;
        //                    }
        //                    if (lsUpdate != null && lsUpdate.Count > 0 && resultExucte)
        //                    {
        //                        resultExucte = DA_Product_Material.Instance.Update(lsUpdate) > 0 ? true : false;
        //                    }
        //                }

        //            }
        //            scope.Complete();
        //            return resultExucte;
        //        }
        //    }
        //    catch (Exception ex) { }
        //    return false;
        //}
        private bool MethodForUpdateEntity(TBL_PRODUCT product, List<TBL_PRODUCT_MATERIAL> lsProductMaterial)
        {
            try
            {
                using (var scope = new TransactionScope())
                {

                    int resultExcuteRow = DA_Food.Instance.Update(product);
                    bool resultExucte = resultExcuteRow == 0 ? false : true;
                    if (resultExucte)
                    {

                        var lsProductMaterialOld = DA_Product_Material.Instance.getEntityBaseIdProduct(product.ProductID);
                        if (lsProductMaterialOld != null && lsProductMaterialOld.Count > 0)
                        {
                            DA_Product_Material.Instance.deleteListEntity(lsProductMaterialOld);
                        }

                        if (lsProductMaterial != null && lsProductMaterial.Count > 0)
                        {
                            DA_Product_Material.Instance.Insert(lsProductMaterial);
                        }

                    }
                    scope.Complete();
                    return resultExucte;
                }
            }
            catch (Exception ex) { return false; }

        }
        /// <summary>
        /// set productId for list product material
        /// </summary>
        /// <param name="lsProductMaterial"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private void setProductIdForListProductMaterial(ref List<TBL_PRODUCT_MATERIAL> lsProductMaterial, int productId)
        {
            if (lsProductMaterial != null && lsProductMaterial.Count > 0)
            {
                foreach (TBL_PRODUCT_MATERIAL item in lsProductMaterial)
                {
                    item.ProductID = productId;
                }
            }
        }

        #endregion
    }
}