using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Service : EfRepositoryBase<TBL_SERVICE>
    {
        #region para
        private static volatile DA_Service _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Service Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Service();
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
        public List<object> getServiceForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
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
                    getData = (from u in context.TBL_SERVICE
                               where ((search == "") || u.ServiceName.Contains(search) || u.Notes.Contains(search) )
                               select new { u.ServiceID, u.ServiceName, u.UnitPrice, u.Notes }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "ServiceID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
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
        public int countAllServiceFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_SERVICE where ((search == "") || u.ServiceName.Contains(search) || u.Notes.Contains(search)) select u).Count();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region For combobox
        public List<Object> loadEntityForCombobox(bool searchActiveEqualTrue)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<Object> result = new List<object>();
                    result = (from u in context.TBL_SERVICE
                              where !searchActiveEqualTrue || u.IsActive
                              select new { u.ServiceID, u.ServiceName }).OrderBy("ServiceName asc").ToList<Object>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion



        public bool deleteEntity(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string queryDeleteService = "DELETE FROM TBL_SERVICE WHERE ServiceID = @id";
                    string queryUpdatePartyService = "UPDATE TBL_PARTY_SERVICE SET ServiceID = 0 WHERE ServiceID = @id";
                    Instance.ExecuteSqlCommand(queryDeleteService, new SqlParameter("@id", id));
                    Instance.ExecuteSqlCommand(queryUpdatePartyService, new SqlParameter("@id", id));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        #endregion
    }
}