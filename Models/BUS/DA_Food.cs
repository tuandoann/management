using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Food : EfRepositoryBase<TBL_PRODUCT>
    {
        #region para
        private static volatile DA_Food _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Food Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Food();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get service for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getFoodForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //check data
                    search = string.IsNullOrWhiteSpace(search) ? "" : search;
                    sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "" : sortColumn;
                    sortColumnDir = string.IsNullOrWhiteSpace(sortColumnDir) ? "" : sortColumnDir;
                    //excute query 
                    getData = (from f in context.TBL_PRODUCT
                               join pf in context.TBL_PRODUCT_GROUP on f.ProductGroupID equals pf.ProductGroupID into pfs
                               from pf in pfs.DefaultIfEmpty()
                               where (search == "" || pf.GroupName.Contains(search) || f.ProductName.Contains(search))
                               select new { GroupName = pf.GroupName == null ? "" : pf.GroupName, f.ProductID, f.ProductName, f.ProfitAmount, f.IsActive })
                               .OrderBy((sortColumn == "" && sortColumnDir == "") ? "GroupName desc" : sortColumn + " " + sortColumnDir)
                               .Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all service flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllFoodFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from f in context.TBL_PRODUCT
                              join pf in context.TBL_PRODUCT_GROUP on f.ProductGroupID equals pf.ProductGroupID into pfs
                              from pf in pfs.DefaultIfEmpty()
                              where (search == "" || pf.GroupName.Contains(search) || f.ProductName.Contains(search))
                              select f).Count();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region for combobox
        public List<object> loadEntityForCombobox(bool searchActiveEqualTrue)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<Object> result = new List<object>();
                    result = (from u in context.TBL_PRODUCT
                              where !searchActiveEqualTrue || u.IsActive
                              select new { u.ProductID, u.ProductName }).OrderBy("ProductName asc").ToList<Object>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }

        #endregion

        /// <summary>
        /// check entity can delete
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool checkProduct(int productID)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                   
                    return (context.TBL_PARTY_PRODUCT_MATERIAL.Where(n => n.ProductID == productID).Count()
                           + context.TBL_PRODUCT_MATERIAL.Where(n => n.ProductID == productID).Count()
                           + context.TBL_PARTY_PRODUCT.Where(n => n.ProductID == productID).Count()) > 0 ? true : false;
                }
            } catch (Exception ex){ return false;}
        }



        #endregion
    }
}