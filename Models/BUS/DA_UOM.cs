using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_UOM : EfRepositoryBase<TBL_UOM>
    {
        #region para
        private static volatile DA_UOM _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_UOM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_UOM();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get UOM for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getUomForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
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
                    getData = (from u in context.TBL_UOM
                               where search == "" || u.UOMName.Contains(search)
                               select u).OrderBy((sortColumn == "" && sortColumnDir == "") ? "UOMID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
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
        public int countAllUomFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_UOM
                              where search == "" || u.UOMName.Contains(search)
                              select u).Count();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region delete
        public bool deleteEntity(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string queryDeleteUom = "DELETE FROM TBL_UOM WHERE UOMID = @id";
                    string queryUpdateMaterial = "UPDATE TBL_MATERIAL SET UOMID = 0 WHERE UOMID = @id";
                    string queryUpdateProductMaterial = "UPDATE TBL_PRODUCT_MATERIAL SET UOMID = 0 WHERE UOMID = @id";
                    Instance.ExecuteSqlCommand(queryDeleteUom, new SqlParameter("@id", id));
                    Instance.ExecuteSqlCommand(queryUpdateMaterial, new SqlParameter("@id", id));
                    Instance.ExecuteSqlCommand(queryUpdateProductMaterial, new SqlParameter("@id", id));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        #endregion

        #endregion

    }
}