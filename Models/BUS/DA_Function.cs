using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Function : EfRepositoryBase<SYS_FUNCTION>
    {
        #region para
        private static volatile DA_Function _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Function Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Function();
                    }
                }
                return _instance;
            }
        }
        #endregion



        #region method

        public List<SYS_FUNCTION> GetMenuFunctionByRole(int userId)
        {
            try
            {
                List<SYS_FUNCTION> result = new List<SYS_FUNCTION>();
                SYS_USER item = DA_User.Instance.GetById(userId);

                var listbool = new List<bool>();
                if(item.IsConfig ==true)
                {
                    listbool.Add(item.IsConfig);
                }
                if (item.IsRegisterParty == true)
                {
                    listbool.Add(item.IsRegisterParty);
                }
                if (item.IsMaterial == true)
                {
                    listbool.Add(item.IsMaterial);
                }
                if (item.IsAttendance == true)
                {
                    listbool.Add(item.IsAttendance);
                }
                if (item.IsList == true)
                {
                    listbool.Add(item.IsList);
                }
                if (item.IsList == true)
                {
                    listbool.Add(item.IsReport);
                }
                
                if (item.IsAdmin == true)
                {
                  return  Instance.GetAll().ToList();

                }
   
             

                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                    {

                    item = from a in context.SYS_FUNCTION where a.ModuleCode ==  fvv in




                        return result;
                    }
            }
            catch (Exception ex) { return null; }
        }

        #endregion
    }
}