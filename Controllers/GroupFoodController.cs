using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;

namespace QUANLYTIEC.Controllers
{
    public class GroupFoodController : BaseController
    {
        // GET: GroupFood
        #region method is used for load page
        public ActionResult Index()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_GROUP).Name, "index");
            List<object> ls = new List<object>();
            DA_GroupFood.Instance.ProductGroupID(ref ls, null);
            ViewBag.List = ls.ToArray();
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_GROUP).Name, "edit");
            if (!string.IsNullOrWhiteSpace(id) && id.All(Char.IsDigit))
            {
                ViewBag.combobox = DA_GroupFood.Instance.GetAllEntityExceptProductGroupId(Convert.ToInt32(id));
                return View(DA_GroupFood.Instance.GetById(Convert.ToInt32(id)));
            }
            return RedirectToAction("Index", "GroupFood");
        }
        public ActionResult Create()
        {
            if (!CheckPermission())
                return RedirectToAction("Index", "Login");
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_GROUP).Name, "create");
            ViewBag.combobox = DA_GroupFood.Instance.GetAll().ToList();
            return View();
        }
        #endregion

        #region method is used for ajax
        /// <summary>
        /// ajax for datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGroupFoodDatatableIndex()
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
                var sortColumn = Request.Form.GetValues("columns[" + (orderColumn == 0 ? 1 : orderColumn) + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //find search columns info
                var search = Request.Form["search[value]"];
                //page
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt16(start) : 0;
                #endregion

                long recordsTotal = 0;

                List<object> data = DA_GroupFood.Instance.getGroupFoodForDatatablePagging(search.ToString(), skip, length != null ? Convert.ToInt32(length) : 0, sortColumn, sortColumnDir);
                recordsTotal = DA_GroupFood.Instance.countAllGroupFoodFlowSearch(search.ToString());
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
                string id = (Request.Form.GetValues("id").FirstOrDefault() == null
                    ? ""
                    : Request.Form.GetValues("id").FirstOrDefault().ToString());

                if (id.All(char.IsDigit))
                {
                    return Json(DA_GroupFood.Instance.deleteEntity(id) ? 1 : 0);
                }
                return Json(0);

            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
        #endregion

        #region method is used for sumbit form
        /// <summary>
        /// method excute save item when sumbit form create
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string groupName, string parentName)
        {
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                try
                {
                    int parentId = (parentName.Split('/').Length > 0) ? Convert.ToInt32(parentName.Split('/')[0].All(Char.IsDigit) ? parentName.Split('/')[0] : "0") : 0;
                    int levelId = (parentName.Split('/').Length > 0) ? Convert.ToInt32(parentName.Split('/')[1].All(Char.IsDigit) ? parentName.Split('/')[1] : "0") : 0;
                    TBL_PRODUCT_GROUP item = new TBL_PRODUCT_GROUP();
                    item.GroupName = groupName;
                    if(parentId > 0)
                        item.ParentID = parentId;
                    item.LevelID = levelId == 0 ? levelId : levelId + 1;
                    DA_GroupFood.Instance.Insert(item);
                    return RedirectToAction("Index", "GroupFood");
                }
                catch (Exception ex) { }
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_GROUP).Name, "create");
            ViewBag.combobox = DA_GroupFood.Instance.GetAll().ToList();
            return View();
        }
        /// <summary>
        /// method excute save item when sumbit form edit
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="serviceName"></param>
        /// <param name="unitPrice"></param>
        /// <param name="notes"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string productGroupID, string groupName, string parentName)
        {
            if (!string.IsNullOrWhiteSpace(groupName) && !string.IsNullOrWhiteSpace(productGroupID) && productGroupID.All(Char.IsDigit))
            {
                try
                {
                    int parentId = (parentName.Split('/').Length > 0) ? Convert.ToInt32(parentName.Split('/')[0].All(Char.IsDigit) ? parentName.Split('/')[0] : "0") : 0;
                    int levelId = (parentName.Split('/').Length > 0) ? Convert.ToInt32(parentName.Split('/')[1].All(Char.IsDigit) ? parentName.Split('/')[1] : "0") : 0;
                    TBL_PRODUCT_GROUP item = DA_GroupFood.Instance.GetById(Convert.ToInt32(productGroupID));
                    item.GroupName = groupName;
                    if (parentId > 0)
                        item.ParentID = parentId;
                    item.LevelID = levelId + 1;
                    DA_GroupFood.Instance.Update(item);
                    return RedirectToAction("Index", "GroupFood");
                }
                catch (Exception ex) { return View(); }
            }
            ViewBag.title = TitleEnum.getTitleForPage(typeof(TBL_PRODUCT_GROUP).Name, "edit");
            ViewBag.combobox = DA_GroupFood.Instance.GetAll().ToList();
            return View();
        }
        #endregion
    }
}