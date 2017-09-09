using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PartyProduct : EfRepositoryBase<TBL_PARTY_PRODUCT>
    {
        #region para
        private static volatile DA_PartyProduct _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PartyProduct Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PartyProduct();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method
        public List<TBL_PARTY_PRODUCT> getEntityBasePartyId(int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PARTY_PRODUCT> result = new List<TBL_PARTY_PRODUCT>();
                    result = context.TBL_PARTY_PRODUCT.Where(n => n.PartyID == partyId).ToList<TBL_PARTY_PRODUCT>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion

    }
}