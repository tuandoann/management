using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Vendor : EfRepositoryBase<TBL_VENDOR>
    {
        #region para
        private static volatile DA_Vendor _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Vendor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Vendor();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Vendor for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getVendorForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
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
                    getData = (from u in context.TBL_VENDOR
                               where search == "" || u.VendorName.Contains(search) || u.HomePhone.Contains(search) || u.PhoneNumber.Contains(search) || u.Address.Contains(search) || u.Address.Contains(search)
                               select new { u.VendorID,u.HomePhone, u.VendorName, u.PhoneNumber, u.Address }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "VendorID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Vendor flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllVendorFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_VENDOR
                               where search == "" || u.VendorName.Contains(search) || u.Address.Contains(search)
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

        #region For combobox
        public List<object> getEntityForCombobox()
        {
            try
            {
                using(var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<Object> result = new List<object>();
                    result = context.TBL_VENDOR.Select(n=>new{n.VendorID,n.VendorName}).ToList<Object>();
                    return result;
                }
            }
            catch (Exception ex) { return null; };
        }
        #endregion

     
        public bool deleteEntity(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string queryDeleteVendor = "DELETE FROM TBL_VENDOR WHERE VendorID = @id";
                    string queryUpdatePartyProductMaterial = "UPDATE TBL_PARTY_PRODUCT_MATERIAL SET VendorID = 0 WHERE VendorID = @id";
                    Instance.ExecuteSqlCommand(queryDeleteVendor, new SqlParameter("@id", id));
                    Instance.ExecuteSqlCommand(queryUpdatePartyProductMaterial, new SqlParameter("@id", id));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        #endregion
    }
}