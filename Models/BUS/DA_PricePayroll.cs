using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Data.Entity.SqlServer;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PricePayroll : EfRepositoryBase<TBL_PRICE_PAYROLL>
    {
        #region para
        private static volatile DA_PricePayroll _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PricePayroll Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PricePayroll();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Price Payroll for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getPricePayrollForDatatablePagging(int start, int length, string sortColumn, string sortColumnDir)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //check data
                    sortColumn = String.IsNullOrWhiteSpace(sortColumn) ? "" : sortColumn;
                    sortColumnDir = String.IsNullOrWhiteSpace(sortColumnDir) ? "" : sortColumnDir;
                    //excute query 
                    getData = (from u in context.TBL_PRICE_PAYROLL
                               select new { u.PriceID, u.UnitPrice, TimeFrom = SqlFunctions.DateName("hour", u.TimeFrom) + ":" + SqlFunctions.DateName("minute", u.TimeFrom), TimeTo = SqlFunctions.DateName("hour", u.TimeTo) + ":" + SqlFunctions.DateName("minute", u.TimeTo) }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "PriceID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Price Payroll flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllPricePayrollFlowSearch()
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //excute query 
                    result = (from u in context.TBL_PRICE_PAYROLL
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


        public bool deleteEntity(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string queryDeletePayroll = "DELETE FROM TBL_PRICE_PAYROLL WHERE PriceID = @id";
                    Instance.ExecuteSqlCommand(queryDeletePayroll, new SqlParameter("@id", id));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        #endregion
    }
}