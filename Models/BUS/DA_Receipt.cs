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
    public class DA_Receipt : EfRepositoryBase<TBL_RECEIPT>
    {
        #region para
        private static volatile DA_Receipt _instance;
        private static readonly object SyncRoot = new object();
        #endregion

        #region Constructor
        public static DA_Receipt Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Receipt();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region For datatable
        /// <summary>
        /// get Employee for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getReceiptForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir, DateTime ValueDateFrom, DateTime ValueDateTo)
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
                    getData = (from u in context.TBL_RECEIPT
                               join d in context.TBL_PARTY
                               on u.PartyID equals d.PartyID into ds
                               from d in ds.DefaultIfEmpty()
                               where (search == "" || d.CustomerName.Contains(search) || u.ReceiptNo.Contains(search) || d.CustomerName.Contains(search)) && (u.ReceiptDate >= ValueDateFrom && u.ReceiptDate <= ValueDateTo)
                               select new
                               {
                                   u.ReceiptID,
                                   u,
                                   d.CustomerName,
                                   u.ReceiptNo,
                                   ReceiptDate = SqlFunctions.DateName("day", u.ReceiptDate)
                               + "/" + SqlFunctions.StringConvert((double)u.ReceiptDate.Month).TrimStart()
                               + "/" + SqlFunctions.DateName("year", u.ReceiptDate),
                                   u.Amount
                               })
                               .OrderBy((sortColumn == "" && sortColumnDir == "") ? "ReceiptID asc" : sortColumn + " " + sortColumnDir).Skip(start)
                               .Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all Department flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllReceiptFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = string.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_RECEIPT
                              join d in context.TBL_PARTY on u.PartyID equals d.PartyID into ds
                              from d in ds.DefaultIfEmpty()
                              where search == "" || u.ReceiptNo.Contains(search) || d.CustomerName.Contains(search)
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
        //load party for edit
        public TBL_PARTY getPartyforEdit(int receiptID)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    TBL_PARTY getData = new TBL_PARTY();
                    getData = (from r in context.TBL_RECEIPT
                               join d in context.TBL_PARTY on r.PartyID equals d.PartyID into ds
                               from d in ds.DefaultIfEmpty()
                               where r.ReceiptID == receiptID
                               select d).FirstOrDefault();
                    return getData;
                }
            }
            catch (Exception ex) { return null; }
        }
        public bool checkReceiptNo(string receiptNo, int receiptID, bool isEdit)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    TBL_RECEIPT getData = new TBL_RECEIPT();
                    if (isEdit)
                    {
                        getData = (from r in context.TBL_RECEIPT
                                   where r.ReceiptID != receiptID && r.ReceiptNo == receiptNo
                                   select r).FirstOrDefault<TBL_RECEIPT>();
                    }
                    else
                    {
                        getData = (from r in context.TBL_RECEIPT
                                   where r.ReceiptNo == receiptNo
                                   select r).FirstOrDefault<TBL_RECEIPT>();
                    }
                    return (getData != null ? true : false);
                }
            }
            catch (Exception ex) { return false; };

        }
    }
    #endregion
}
