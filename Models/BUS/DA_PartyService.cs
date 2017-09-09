using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_PartyService : EfRepositoryBase<TBL_PARTY_SERVICE>
    {
        #region para
        private static volatile DA_PartyService _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_PartyService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_PartyService();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method
        public List<TBL_PARTY_SERVICE> getEntityBasePartyId(int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PARTY_SERVICE> result = new List<TBL_PARTY_SERVICE>();
                    result = context.TBL_PARTY_SERVICE.Where(n => n.PartyID == partyId).ToList<TBL_PARTY_SERVICE>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion
    }
}