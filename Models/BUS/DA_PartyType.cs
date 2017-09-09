using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PartyType : EfRepositoryBase<SYS_PARTY_TYPE>
    {
        #region para
        private static volatile DA_PartyType _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PartyType Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PartyType();
                    }
                }
                return _instance;
            }
        }
        #endregion
        
    }
}