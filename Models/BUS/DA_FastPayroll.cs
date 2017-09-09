using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_FastPayroll : EfRepositoryBase<TBL_FAST_PAYROLL>
    {
        #region para
        private static volatile DA_FastPayroll _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_FastPayroll Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_FastPayroll();
                    }
                }
                return _instance;
            }
        }
        #endregion
    }
}