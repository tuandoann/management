using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_User : EfRepositoryBase<SYS_USER>
    {
        #region para
        private static volatile DA_User _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_User Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_User();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method
        /// <summary>
        /// get user base name and id
        /// </summary>
        /// <param name="user">user name</param>
        /// <param name="pass">pass</param>
        /// <returns></returns>
        /// 
        public SYS_USER GetPermissionByFunction(int userID, bool IsAdmin = false)
        {
            if (IsAdmin)
            {
                return new SYS_USER
                {
                    IsConfig = true,
                    IsRegisterParty = true,
                    IsMaterial = true,
                    IsAttendance = true,
                    IsList = true,
                    IsReport = true
                };
            }

            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                SYS_USER item = context.SYS_USER.FirstOrDefault(x => x.UserID == userID);
                if (item != null)
                {
                    return item;
                }
            }
            return new SYS_USER
            {
                IsConfig = false,
                IsRegisterParty = false,
                IsMaterial = false,
                IsAttendance = false,
                IsList = false,
                IsReport = false
            };
        }





        public SYS_USER getUserBaseNameAndPass(string user, string pass)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var item = context.SYS_USER.SingleOrDefault(n => n.Password == pass && n.UserName == user);
                return item;
            }
        }
        /// <summary>
        /// get item base user name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SYS_USER getItemBasetUserName(string user)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                SYS_USER item = context.SYS_USER.Where(n => n.UserName == user).FirstOrDefault();
                return item;
            }
        }
        #region For datatable
        /// <summary>
        /// get user for datatable flow pagging
        /// </summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortColumnDir"></param>
        /// <returns></returns>
        public List<object> getUserForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir)
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
                    getData = (from u in context.SYS_USER
                               where ((search == "") || u.FullName.Contains(search) || u.UserName.Contains(search) || u.Email.Contains(search))
                               select new { u.UserID, u.FullName, u.UserName, u.Email }).OrderBy((sortColumn == "" && sortColumnDir == "") ? "UserID asc" : sortColumn + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        /// <summary>
        /// get all user flow search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int countAllUserFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.SYS_USER
                              where ((search == "") || u.FullName.Contains(search) || u.UserName.Contains(search) || u.Email.Contains(search))
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
        #endregion

    }
}