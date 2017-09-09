using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PartyProductMaterial : EfRepositoryBase<TBL_PARTY_PRODUCT_MATERIAL>
    {
        #region para
        private static volatile DA_PartyProductMaterial _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PartyProductMaterial Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PartyProductMaterial();
                    }
                }
                return _instance;
            }
        }
        #endregion
    }
}