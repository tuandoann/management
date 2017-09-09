using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Data.Entity;
using ScipBe.Common.LinqExtensions;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_GroupFood : EfRepositoryBase<TBL_PRODUCT_GROUP>
    {
        #region para
        private static volatile DA_GroupFood _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_GroupFood Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_GroupFood();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Group Food for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getGroupFoodForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    sortColumn = String.IsNullOrWhiteSpace(sortColumn) ? "" : sortColumn;
                    sortColumnDir = String.IsNullOrWhiteSpace(sortColumnDir) ? "" : sortColumnDir;
                    //excute query 
                    getData = (from u in context.TBL_PRODUCT_GROUP
                               join u1 in context.TBL_PRODUCT_GROUP on u.ParentID equals u1.ProductGroupID into us
                               from u1 in us.DefaultIfEmpty()
                               where search == "" || u.GroupName.Contains(search) || u1.GroupName.Contains(search)
                               select new { u.ProductGroupID, u.GroupName, ParentName = u1.GroupName == null ? "" : u1.GroupName, u.ParentID }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "ProductGroupID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<Object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Group Food flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllGroupFoodFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_PRODUCT_GROUP
                              join u1 in context.TBL_PRODUCT_GROUP on u.ParentID equals u1.ProductGroupID into us
                              from u1 in us.DefaultIfEmpty()
                              where search == "" || u.GroupName.Contains(search) || u1.GroupName.Contains(search)
                              select new { u.ProductGroupID, u.GroupName, ParentName = u1.GroupName == null ? "" : u1.GroupName }).Count();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region delete entity
        /// <summary>
        /// delete entity groupFood base id
        /// </summary>
        /// <param name="id">id entity</param>
        /// <returns></returns>
        public bool deleteEntity(string id)
        {

            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                try
                {
                    int ID = Convert.ToInt32(id);
                    List<TBL_PRODUCT_GROUP> ds = context.TBL_PRODUCT_GROUP.ToList<TBL_PRODUCT_GROUP>();
                    List<TBL_PRODUCT_GROUP> ds1 = ds.Where(n => n.ProductGroupID != ID).ToList();
                    //delete entity is ProductGroupID = id
                    TBL_PRODUCT_GROUP itemDelete = ds.SingleOrDefault(n => n.ProductGroupID == ID);
                    if (_instance.Delete(itemDelete) <= 0 && itemDelete != null)
                        return false;
                    //update all entity food is ParentID = id
                    if (!removeParentForChildItem(ID, true, ref ds))
                    {
                        _instance.Insert(itemDelete);
                        _instance.Update(ds1);
                        return false;
                    }
                    //update all entity is ProductGroupID = id
                    List<TBL_PRODUCT> dsProduct = context.TBL_PRODUCT.Where(n => n.ProductGroupID == ID).ToList<TBL_PRODUCT>();
                    foreach (TBL_PRODUCT item in dsProduct)
                    {
                        item.ProductGroupID = 0;
                    }
                    if (DA_Food.Instance.Update(dsProduct) <= 0 && dsProduct.Count > 0)
                    {
                        _instance.Insert(itemDelete);
                        _instance.Update(ds1);
                    }

                    return true;
                }

                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// dùng để update đệ quy khi có một thằng cha bị xóa thì tất cả thằng con phải update với client id - 1, và thằng cấp thằng cha thì parent id = 0
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="isLevelHighest"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        private bool removeParentForChildItem(int ID, bool isLevelHighest, ref List<TBL_PRODUCT_GROUP> ds)
        {
            try
            {
                List<TBL_PRODUCT_GROUP> dsContaintID = ds.Where(n => Convert.ToInt32(n.ParentID) == ID).ToList();
                if (ds.RemoveAll(n => n.ParentID == ID) <= 0 && dsContaintID.Count > 0)
                    return false;
                if (dsContaintID.Count > 0)
                {
                    foreach (TBL_PRODUCT_GROUP item in dsContaintID)
                    {
                        //update data item
                        item.ParentID = isLevelHighest ? null : item.ParentID;
                        --item.LevelID;
                        if (_instance.Update(item) <= 0)
                            return false;
                        //đệ quy
                        if (!removeParentForChildItem(item.ProductGroupID, false, ref ds))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }
        #endregion

        /// <summary>
        /// method select get entity is flow no child
        /// </summary>
        /// <returns></returns>
        public List<TBL_PRODUCT_GROUP> getAllEntityNoChild()
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PRODUCT_GROUP> result = new List<TBL_PRODUCT_GROUP>();
                    result = (from pg in context.TBL_PRODUCT_GROUP
                              where !(from pg1 in context.TBL_PRODUCT_GROUP select pg1.ParentID).Contains(pg.ProductGroupID)
                              select pg).ToList<TBL_PRODUCT_GROUP>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }

        /// <summary>
        /// get all entity except product groupd id
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public List<TBL_PRODUCT_GROUP> GetAllEntityExceptProductGroupId(int ProductGroupId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PRODUCT_GROUP> result = new List<TBL_PRODUCT_GROUP>();
                    result = (from pg in context.TBL_PRODUCT_GROUP
                              where pg.ProductGroupID != ProductGroupId
                              select pg).ToList<TBL_PRODUCT_GROUP>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        public void ProductGroupID(ref List<object> result, long? ParentID)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var ds = from b in context.TBL_PRODUCT_GROUP
                         where b.ParentID == ParentID
                         select b.ProductGroupID;
                if (ds.Any())
                {
                    foreach (long id in ds)
                    {
                        // lay thong tin node hien tai:
                        Object child = (from u in context.TBL_PRODUCT_GROUP
                                                   join u1 in context.TBL_PRODUCT_GROUP on u.ParentID equals u1.ProductGroupID into us
                                                   from u1 in us.DefaultIfEmpty()
                                                   where u.ProductGroupID == id
                                                   select new { u.ProductGroupID, u.GroupName, ParentName = u1.GroupName == null ? "" : u1.GroupName, u.ParentID }).FirstOrDefault();

                        if (child != null)
                        {
                            result.Add(child);
                            ProductGroupID(ref result, Convert.ToInt32(child.GetType().GetProperty("ProductGroupID").GetValue(child, null)));
                        }
                    }
                }
            }
        }
        #endregion

    }
}