using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Transactions;
using System.Data.SqlClient;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Material : EfRepositoryBase<TBL_MATERIAL>
    {
        #region para
        private static volatile DA_Material _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Material Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Material();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Material for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getMaterialForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
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
                    getData = (from u in context.TBL_MATERIAL
                               join n in context.TBL_UOM on u.UOMID equals n.UOMID into lsN
                               from n in lsN.DefaultIfEmpty()
                               where search == "" || u.MaterialName.Contains(search) || u.Notes.Contains(search) || n.UOMName.Contains(search)
                               select new { u.MaterialID, u.MaterialName, u.Notes, UOMName = n.UOMName, u.UnitPrie }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "MaterialID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Material flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllMaterialFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_MATERIAL
                              where search == "" || u.MaterialName.Contains(search) || u.Notes.Contains(search)
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


        public bool UsingMaterial(int materialID)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {

                    return (context.TBL_PRODUCT_MATERIAL.Where(n => n.MaterialID == materialID).Count()
                           + context.TBL_PARTY_PRODUCT_MATERIAL.Where(n => n.MaterialID == materialID).Count()) > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      
        #endregion

    }
}