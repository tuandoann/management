using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq.Dynamic;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Party : EfRepositoryBase<TBL_PARTY>
    {
        #region para
        private static volatile DA_Party _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Party Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Party();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        #region full calendar
        /// <summary>
        /// method get entity base ranger party date
        /// </summary>
        /// <param name="PartyDateFrom">party date start</param>
        /// <param name="PartyDateTo">party date finish</param>
        /// <returns></returns>
        public List<Object> GetEntityBasePartyDate(DateTime PartyDateFrom, DateTime PartyDateTo)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<Object> result = new List<Object>();
                    result = (from u in context.TBL_PARTY
                              where u.PartyDate >= PartyDateFrom && u.PartyDate <= PartyDateTo
                              select new { u.PartyID, u.CustomerName, u.PartyAddress,
                                  PartyDate = SqlFunctions.DateName("day", u.PartyDate) 
                                  + "/" + SqlFunctions.DateName("month", u.PartyDate) 
                                  + "/" + SqlFunctions.DateName("year", u.PartyDate)
                                  + " " + SqlFunctions.DateName("hour", u.PartyDate)
                                  + ":" + SqlFunctions.DateName("minute", u.PartyDate) }).ToList<Object>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion

        #region exucute delete, update, insert entity
        /// <summary>
        /// excute insert entity
        /// </summary>
        /// <param name="objectParty"></param>
        /// <param name="lsObjectMeal"></param>
        /// <param name="lsObjecService"></param>
        /// <returns></returns>
        public bool ProcesseActionInsertFormAllEntity(TBL_PARTY objectParty, List<TBL_PARTY_PRODUCT> lsObjectMeal, List<TBL_PARTY_SERVICE> lsObjecService)
        {
            try
            {
                //insert object Party
                int resultExcute = Instance.Insert(objectParty);
                if (resultExcute > 0)
                {
                        var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr);
                        using (context)
                        {
                            setValueForAttributeListClassObject(objectParty.PartyID, ref lsObjectMeal, "PartyID");
                            setValueForAttributeListClassObject(objectParty.PartyID, ref lsObjecService, "PartyID");
                            var dbSetInsertMeal = context.Set<TBL_PARTY_PRODUCT>();
                            addObjectToDBSet_Insert(ref context, ref dbSetInsertMeal, lsObjectMeal);
                            var dbSetInsertService = context.Set<TBL_PARTY_SERVICE>();
                            addObjectToDBSet_Insert(ref context, ref dbSetInsertService, lsObjecService);
                            int result = context.SaveChanges();
                            return ( result > 0 || (lsObjecService == null && lsObjectMeal == null && result == 0)) ? true : false;
                        }
                }
            }
            catch (Exception ex) { }
            return false;
        }
        /// <summary>
        /// excute update entity
        /// </summary>
        /// <param name="objectParty"></param>
        /// <param name="lsObjectMeal"></param>
        /// <param name="lsObjecService"></param>
        /// <returns></returns>
        public bool ProcesseActionUpdateFormAllEntity(TBL_PARTY objectParty, List<TBL_PARTY_PRODUCT> lsObjectMeal, List<TBL_PARTY_SERVICE> lsObjecService)
        {
            try
            {
                //insert object Party
                int resultExcute = Instance.Update(objectParty);
                if (resultExcute > 0)
                {
                    var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr);
                    using (context)
                    {
                        int countDSMealOld = 0;
                        int countDsServerOld = 0;
                        ProcesseActionUpdateFormEnityMeal(ref context, objectParty, lsObjectMeal, ref countDSMealOld);
                        ProcesseActionUpdateFormEnityService(ref context, objectParty, lsObjecService, ref countDsServerOld);
                        int result = context.SaveChanges();
                        return (result > 0 || (result == 0  &&  lsObjecService == null && lsObjectMeal == null && countDSMealOld == 0 && countDsServerOld == 0 ) )? true : false;
                    }
                }
            }
            catch (Exception ex) { }
            return false;
        }

        /// <summary>
        /// excute sql delete entity party
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public bool ProcesseActionDeleteFormAllEntity(int partyId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string querySqlDetelePartyItem = "DELETE FROM TBL_PARTY WHERE PartyID = @Id";
                    string querySqlDetelePartyProduct = "DELETE FROM TBL_PARTY_PRODUCT WHERE PartyID = @Id";
                    string querySqlDetelePartyService = "DELETE FROM TBL_PARTY_SERVICE WHERE PartyID = @Id";
                    string querySqlDeletePartyProductMaterial = "DELETE FROM TBL_PARTY_PRODUCT_MATERIAL WHERE PartyID = @Id";
                    DA_Party.Instance.ExecuteSqlCommand(querySqlDetelePartyItem, new SqlParameter("@Id", partyId));
                    DA_PartyProduct.Instance.ExecuteSqlCommand(querySqlDetelePartyProduct, new SqlParameter("@Id", partyId));
                    DA_PartyService.Instance.ExecuteSqlCommand(querySqlDetelePartyService, new SqlParameter("@Id", partyId));
                    DA_PartyService.Instance.ExecuteSqlCommand(querySqlDeletePartyProductMaterial, new SqlParameter("@Id", partyId));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }
        #endregion

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
        public List<object> getPartyForDatatablePagging(string search, int start, int length, string sortColumn, string sortColumnDir, bool isSearchDate, DateTime valueDate)
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
                    DateTime startDate = new DateTime(valueDate.Year, valueDate.Month, valueDate.Day, 0, 0, 0, 0);
                    DateTime endDate = new DateTime(valueDate.Year, valueDate.Month, valueDate.Day, 23, 59, 59, 999);

                    //excute query 
                    getData = (from u in context.TBL_PARTY
                               where (!isSearchDate && (search == "" || u.CustomerName.Contains(search))
                               || (isSearchDate && u.PartyDate >= startDate && u.PartyDate <= endDate))
                               select new { u.PartyID,
                                   PartyDate = SqlFunctions.DateName("day", u.PartyDate)
                               + "/" + SqlFunctions.StringConvert((double)u.PartyDate.Month).TrimStart()
                               + "/" + SqlFunctions.DateName("year", u.PartyDate)
                               + " " + SqlFunctions.DateName("hour", u.PartyDate) 
                               + ":" + SqlFunctions.DateName("minute", u.PartyDate), u.CustomerName, u.NumberTablePlan, u.NumberTableException, u.NumberTableVegetarian })
                               .OrderBy((sortColumn == "" && sortColumnDir == "") ? "CustomerName asc" : sortColumn 
                               + " " + sortColumnDir).Skip(start).Take(length).ToList<object>();
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
        public int countAllPartyFlowSearch(string search)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    int result = 0;
                    //check data
                    search = String.IsNullOrWhiteSpace(search) ? "" : search;
                    //excute query 
                    result = (from u in context.TBL_PARTY
                              where search == "" || u.CustomerName.Contains(search)
                              select u).Count();
                    return result;
                }
            }
            catch (Exception ex) { return 0; }
        }
        #endregion

        public List<object> MethodFillDataWhenClickGetData(int productId, int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //excute query 
                    getData = (from t in (from n in context.TBL_PRODUCT_MATERIAL
                                          where n.ProductID == productId
                                          select new { MaterialID = n.MaterialID, Quantity = n.Quantity, UnitPrice = 0, IsDelivery = false, IdPartyProductMaterial = 0, VendorID = 0 })
                               join m in context.TBL_MATERIAL on t.MaterialID equals m.MaterialID into lsM
                               from m in lsM.DefaultIfEmpty()
                               join u in context.TBL_UOM on m.UOMID equals u.UOMID into lsU
                               from u in lsU.DefaultIfEmpty()
                               select new { m.MaterialID, m.MaterialName, t.Quantity, u.UOMName, UnitPrice = t.IdPartyProductMaterial == 0 ? m.UnitPrie : t.UnitPrice, t.VendorID, t.IsDelivery, t.IdPartyProductMaterial }).ToList<object>();

                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        public List<Object> MethodFillDataForDatatablesProductMaterial(int productId, int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> getData = new List<object>();
                    //excute query 
                    getData = (from t in (from n in context.TBL_PARTY_PRODUCT_MATERIAL
                                          where n.PartyID == partyId && n.ProductID == productId
                                          select new { MaterialID = n.MaterialID, Quantity = n.Quantity, UnitPrice = n.UnitPrice, IsDelivery = n.IsDelivery, IdPartyProductMaterial = n.ID, VendorID = n.VendorID ?? 0 })
                               join m in context.TBL_MATERIAL on t.MaterialID equals m.MaterialID into lsM
                               from m in lsM.DefaultIfEmpty()
                               join u in context.TBL_UOM on m.UOMID equals u.UOMID into lsU
                               from u in lsU.DefaultIfEmpty()
                               select new { m.MaterialID, m.MaterialName, t.Quantity, u.UOMName, UnitPrice = t.IdPartyProductMaterial == 0 ? m.UnitPrie : t.UnitPrice, t.VendorID, t.IsDelivery, t.IdPartyProductMaterial }).ToList<object>();

                    return getData;
                }
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
        public List<object> GetIdAndCountProductInParty(int partyId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<object> result = new List<object>();
                    result = (from p_p in context.TBL_PARTY_PRODUCT
                              join p in context.TBL_PRODUCT on p_p.ProductID equals p.ProductID into lsP_P
                              from p in lsP_P.DefaultIfEmpty()
                              where p_p.PartyID == partyId
                              select new { p.ProductID, p.ProductName }).ToList<object>();
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        private void ProcesseActionUpdateFormEnityMeal(ref ConnectionEFDataFirst context, TBL_PARTY objectParty, List<TBL_PARTY_PRODUCT> lsMeal, ref int countDSOld)
        {
            bool resultExucte = true;
           // setValueForAttributeListClassObject(objectParty.PartyID, ref lsMeal, "PartyID");
            //get list object old of entity product
            List<TBL_PARTY_PRODUCT> lsMealOld = DA_PartyProduct.Instance.getEntityBasePartyId(objectParty.PartyID);
            lsMealOld = lsMealOld == null ? new List<TBL_PARTY_PRODUCT>() : lsMealOld;
            List<TBL_PARTY_PRODUCT> lsMealNew = lsMeal == null ? new List<TBL_PARTY_PRODUCT>() : lsMeal;
            countDSOld = lsMealOld.Count();
            if (lsMealNew.Count > 0 && lsMealOld.Count == 0)
            {
                var dbSetInsertMeal = context.Set<TBL_PARTY_PRODUCT>();
                addObjectToDBSet_Insert(ref context, ref dbSetInsertMeal, lsMealNew);
            }
            if (lsMealNew.Count == 0 && lsMealOld.Count > 0)
            {
                var dbSetDeleteMeal = context.Set<TBL_PARTY_PRODUCT>();
                deleteListObject(ref context, ref dbSetDeleteMeal, lsMealOld);
            }
            if (lsMealNew.Count > 0 && lsMealOld.Count > 0)
            {
                List<int> lsPartyIdNew = lsMealNew.Select(n => n.PartyProductID).ToList();
                List<int> lsPartyIdOld = lsMealOld.Select(n => n.PartyProductID).ToList();
                //array productId 
                List<int> partyIdDelete = lsPartyIdOld.Except(lsPartyIdNew).ToList();
                List<int> partyIdUpdate = lsPartyIdNew.Where(n => lsPartyIdOld.Any(p => p == n)).ToList();

                //item exist array old not exist array new is array delete
                List<TBL_PARTY_PRODUCT> lsDelete = lsMealOld.Where(n => partyIdDelete.Any(p => p == n.PartyProductID)).ToList();
                //item exist array new not exist array old is array delete
                List<TBL_PARTY_PRODUCT> lsInsert = lsMealNew.Where(n => n.PartyProductID == 0).ToList();
                // item exist array new and array old is array insert
                List<TBL_PARTY_PRODUCT> lsUpdate = lsMealNew.Where(n => partyIdUpdate.Any(p => p == n.PartyProductID)).ToList();
                //excute
                if (lsDelete != null && lsDelete.Count > 0)
                {
                    var dbSetDeleteMeal = context.Set<TBL_PARTY_PRODUCT>();
                    deleteListObject(ref context, ref dbSetDeleteMeal, lsDelete);
                }
                if (lsInsert != null && lsInsert.Count > 0 && resultExucte)
                {
                    var dbSetInsertMeal = context.Set<TBL_PARTY_PRODUCT>();
                    addObjectToDBSet_Insert(ref context, ref dbSetInsertMeal, lsInsert);
                }
                if (lsUpdate != null && lsUpdate.Count > 0 && resultExucte)
                {
                    var dbSetUpdateMeal = context.Set<TBL_PARTY_PRODUCT>();
                    addObjectToDBSet_Update(ref context, ref dbSetUpdateMeal, lsUpdate);
                }
            }
        }
        private void ProcesseActionUpdateFormEnityService(ref ConnectionEFDataFirst context, TBL_PARTY objectParty, List<TBL_PARTY_SERVICE> lsService, ref int countDSOld)
        {
            bool resultExucte = true;
            setValueForAttributeListClassObject(objectParty.PartyID, ref lsService, "PartyID");
            //get list object old of entity product
            List<TBL_PARTY_SERVICE> lsServiceOld = DA_PartyService.Instance.getEntityBasePartyId(objectParty.PartyID);
            lsServiceOld = lsServiceOld == null ? new List<TBL_PARTY_SERVICE>() : lsServiceOld;
            List<TBL_PARTY_SERVICE> lsServiceNew = lsService == null ? new List<TBL_PARTY_SERVICE>() : lsService;
            countDSOld = lsServiceOld.Count();
            if (lsServiceNew.Count > 0 && lsServiceOld.Count == 0)
            {
                var dbSetInsertMeal = context.Set<TBL_PARTY_SERVICE>();
                addObjectToDBSet_Insert(ref context, ref dbSetInsertMeal, lsServiceNew);
            }
            if (lsServiceNew.Count == 0 && lsServiceOld.Count > 0)
            {
                var dbSetDeleteMeal = context.Set<TBL_PARTY_SERVICE>();
                deleteListObject(ref context, ref dbSetDeleteMeal, lsServiceOld);
            }
            if (lsServiceNew.Count > 0 && lsServiceOld.Count > 0)
            {
                List<int> lsPartyIdNew = lsServiceNew.Select(n => n.PartyServiceID).ToList();
                List<int> lsPartyIdOld = lsServiceOld.Select(n => n.PartyServiceID).ToList();
                //array productId 
                List<int> partyIdDelete = lsPartyIdOld.Except(lsPartyIdNew).ToList();
                List<int> partyIdUpdate = lsPartyIdNew.Where(n => lsPartyIdOld.Any(p => p == n)).ToList();

                //item exist array old not exist array new is array delete
                List<TBL_PARTY_SERVICE> lsDelete = lsServiceOld.Where(n => partyIdDelete.Any(p => p == n.PartyServiceID)).ToList();
                //item exist array new not exist array old is array delete
                List<TBL_PARTY_SERVICE> lsInsert = lsServiceNew.Where(n => n.PartyServiceID == 0).ToList();
                // item exist array new and array old is array insert
                List<TBL_PARTY_SERVICE> lsUpdate = lsServiceNew.Where(n => partyIdUpdate.Any(p => p == n.PartyServiceID)).ToList();
                //excute
                if (lsDelete != null && lsDelete.Count > 0)
                {
                    var dbSetDeleteMeal = context.Set<TBL_PARTY_SERVICE>();
                    deleteListObject(ref context, ref dbSetDeleteMeal, lsDelete);
                }
                if (lsInsert != null && lsInsert.Count > 0 && resultExucte)
                {
                    var dbSetInsertMeal = context.Set<TBL_PARTY_SERVICE>();
                    addObjectToDBSet_Insert(ref context, ref dbSetInsertMeal, lsInsert);
                }
                if (lsUpdate != null && lsUpdate.Count > 0 && resultExucte)
                {
                    var dbSetUpdateMeal = context.Set<TBL_PARTY_SERVICE>();
                    addObjectToDBSet_Update(ref context, ref dbSetUpdateMeal, lsUpdate);
                }
            }
        }
        public List<object> GetUnitCostAndProfitAmountBaseProductID(int productId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<Object> result = new List<object>();
                    bool excit = context.TBL_PRODUCT_MATERIAL.Select(n => n.ProductID).Contains(productId);
                    long gV = 0;
                    if(excit)
                     gV= Convert.ToInt64((from p_m in context.TBL_PRODUCT_MATERIAL
                                                          join m in context.TBL_MATERIAL on p_m.MaterialID equals m.MaterialID into ls_m
                                                          from m in ls_m.DefaultIfEmpty()
                                                          where p_m.ProductID == productId
                                                          select new { ValueItem = p_m.Quantity * m.UnitPrie }).Sum(n =>  n.ValueItem));

                    int profitAmount = Convert.ToInt32((from p in context.TBL_PRODUCT
                                                            where p.ProductID == productId select p.ProfitAmount).FirstOrDefault());

                    result.Add( new { GV = gV, ProfitAmount = profitAmount });
                    return result;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion
    }
}