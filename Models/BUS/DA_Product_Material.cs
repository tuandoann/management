using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Transactions;

namespace QUANLYTIEC.Models.BUS
{
    public class DA_Product_Material : EfRepositoryBase<TBL_PRODUCT_MATERIAL>
    {
        #region para
        private static volatile DA_Product_Material _instance;
        private static readonly object SyncRoot = new Object();
        #endregion

        #region Constructor
        public static DA_Product_Material Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DA_Product_Material();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region method

        /// <summary>
        /// get entity base id product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<TBL_PRODUCT_MATERIAL> getEntityBaseIdProduct(int productId)
        {
            try
            {
                using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
                {
                    List<TBL_PRODUCT_MATERIAL> result = new List<TBL_PRODUCT_MATERIAL>();
                    result = context.TBL_PRODUCT_MATERIAL.Where(n => n.ProductID == productId).ToList<TBL_PRODUCT_MATERIAL>();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// delete list entity base product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool deleteListEntityBaseProductId(int productId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    string queryDeleteProductMaterial = "DELETE FROM TBL_PRODUCT_MATERIAL WHERE ProductID = @Id";
                    string queryDeleteProduct = "DELETE FROM TBL_PRODUCT WHERE ProductID = @Id";
                    Instance.ExecuteSqlCommand(queryDeleteProduct, new SqlParameter("@Id", productId));
                    Instance.ExecuteSqlCommand(queryDeleteProductMaterial, new SqlParameter("@Id", productId));
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// delete list entity product material
        /// </summary>
        /// <param name="lsEntity"></param>
        /// <returns></returns>
        public bool deleteListEntity(List<TBL_PRODUCT_MATERIAL> lsEntity)
        {
            try
            {
                string query = "DELETE FROM TBL_PRODUCT_MATERIAL WHERE ";
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (TBL_PRODUCT_MATERIAL item in lsEntity)
                {
                    int index = lsEntity.IndexOf(item);
                    query += (index == 0 ? "" : " OR ") + "ProductMaterialID = " + item.ProductMaterialID;
                }
                return Instance.ExecuteSqlCommand(query) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}