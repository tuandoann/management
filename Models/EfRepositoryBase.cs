using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
namespace QUANLYTIEC.Models
{
    public abstract class EfRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected EfRepositoryBase()
        {
            _connectionStr = ConfigurationManager.ConnectionStrings["ConnectionEFDataFirst"].Name;
        }

        public static string _connectionStr;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert(TEntity entity)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Add(entity);
                return context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(TEntity entity)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Delete(TEntity entity)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Insert(IList<TEntity> listEntity)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    dbSet.Add(entity);
                }
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Update(IList<TEntity> listEntity)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    dbSet.Attach(entity);
                    context.Entry(entity).State = EntityState.Modified;
                }
                return context.SaveChanges();
            }
        }



        public virtual int UpdateAdvance(IList<TEntity> listEntity, Expression<TEntity> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    //var a = Find(predicate);
                    //if (a != null)
                    //{
                    //    if (a.Any())
                    //    {
                    //        dbSet.Attach(entity);
                    //        context.Entry(entity).State = EntityState.Modified;
                    //    }
                    //    else
                    //    {
                    //        dbSet.Attach(entity);
                    //        context.Entry(entity).State = EntityState.Added;
                    //    }
                    //}
                    //else
                    //{
                    //    dbSet.Attach(entity);
                    //    context.Entry(entity).State = EntityState.Added;
                    //}

                }
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Delete(params object[] parameters)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                TEntity entity = GetById(parameters);
                if (entity == null)
                {
                    return -1;
                }
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IList<TEntity> GetAll()
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {

                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().FirstOrDefault(predicate);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().SingleOrDefault(predicate);
            }
        }
        /// <summary>
        /// Return an entity with its additional reference objects, eg. User includes Roles, Permissions...
        /// </summary>
        /// <param name="predicate">A where condition to filer for entity</param>
        /// <param name="referenceProperties">The related entity names of current entity, eg. User with related Roles, Permissions...</param>
        /// <returns></returns>
        public TEntity SingleLoadWithReferences(Expression<Func<TEntity, bool>> predicate, params string[] referenceProperties)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                TEntity entity = dbSet.SingleOrDefault(predicate);
                if (entity != null)
                {
                    var properties = typeof(TEntity).GetProperties();
                    foreach (string s in referenceProperties)
                    {
                        var pi = properties.SingleOrDefault(p => p.Name == s);
                        if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                            context.Entry(entity).Collection(s).Load();
                        else
                            context.Entry(entity).Reference(s).Load();
                    }
                }
                return entity;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().Where(predicate).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wherePredicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderPredicate"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public virtual IList<TEntity> SelectWithPaging(Expression<Func<TEntity, bool>> wherePredicate, int? pageIndex, int? pageSize, Func<TEntity, object> orderPredicate, bool? desc)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                IEnumerable<TEntity> entities;
                pageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
                pageSize = pageSize.HasValue ? pageSize.Value : 20;
                desc = desc.HasValue && desc.Value;
                if (wherePredicate != null && orderPredicate == null)
                    entities = dbSet.AsNoTracking().Where(wherePredicate);
                else if (wherePredicate == null && orderPredicate != null)
                    entities = dbSet.AsNoTracking().OrderBy(orderPredicate);
                else
                    entities = dbSet.AsNoTracking().Where(wherePredicate).OrderBy(orderPredicate);

                if ((bool)desc)
                    entities = entities.Reverse();
                if (pageIndex == 0)
                    entities = entities.Take((int)pageSize);
                else
                    entities = entities.Skip((int)pageIndex * (int)pageSize).Take((int)pageSize);

                return entities.ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string commandStr, params object[] parameters)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                return context.Database.ExecuteSqlCommand(commandStr, parameters);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteSqlQuery(string commandStr, params object[] parameters)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                IDbCommand storeQuery = context.Database.Connection.CreateCommand();
                storeQuery.CommandText = commandStr;
                storeQuery.CommandType = CommandType.Text;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeQuery.Parameters.Add(parameter);
                    }
                }
                context.Database.Connection.Open();
                var entities = Activator.CreateInstance<List<TEntity>>();
                using (var reader = storeQuery.ExecuteReader())
                {
                    var properties = typeof(TEntity).GetProperties();
                    while (reader.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(String)) continue;
                            try
                            {
                                prop.SetValue(entity, reader[prop.Name], null);
                            }
                            catch { }
                        }
                        entities.Add(entity);
                    }
                }
                context.Database.Connection.Close();
                return entities;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteStoreCommand(string commandStr, params object[] parameters)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                IDbCommand storeCommand = context.Database.Connection.CreateCommand();
                storeCommand.CommandText = commandStr;
                storeCommand.CommandType = CommandType.StoredProcedure;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeCommand.Parameters.Add(parameter);
                    }
                }
                return storeCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable Exec(string commandStr, params object[] parameters)
        {
            string conn1 = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection cnn = new SqlConnection(conn1);
            if (cnn.State != ConnectionState.Open)
                cnn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(commandStr, cnn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            adapter.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cnn.Close();
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteStoreQuery(string commandStr, params object[] parameters)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                IDbCommand storeQuery = context.Database.Connection.CreateCommand();
                storeQuery.CommandText = commandStr;
                storeQuery.CommandType = CommandType.StoredProcedure;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeQuery.Parameters.Add(parameter);
                    }
                }
                context.Database.Connection.Open();
                var entities = Activator.CreateInstance<List<TEntity>>();
                using (var reader = storeQuery.ExecuteReader())
                {
                    var properties = typeof(TEntity).GetProperties();
                    while (reader.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(String)) continue;
                            try
                            {
                                prop.SetValue(entity, reader[prop.Name], null);
                            }
                            catch { }
                        }
                        entities.Add(entity);
                    }
                }
                context.Database.Connection.Close();
                return entities;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Count();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Count(predicate);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById(params object[] id)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Find(id);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Max(Expression<Func<TEntity, int>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Max(predicate);
            }
        }

        public virtual long? MaxLong(Expression<Func<TEntity, Nullable<long>>> predicate)
        {
            using (var context = (ConnectionEFDataFirst)Activator.CreateInstance(typeof(ConnectionEFDataFirst), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Max(predicate);
            }
        }

        public virtual List<Guid> GetGuidFromString(string str)
        {
            List<Guid> listStore = null;
            if (!string.IsNullOrWhiteSpace(str))
            {
                var getS = str.Split(';');
                if (getS[0] != "")
                {
                    listStore = new List<Guid>();
                    getS.ToList().ForEach(p =>
                    {
                        try
                        {
                            listStore.Add(new Guid(p));
                        }
                        catch
                        {
                        }
                    });
                }
            }
            return listStore;
        }
        protected void deleteListObject<NameClass>(ref ConnectionEFDataFirst context, ref DbSet<NameClass> dbSetObject, List<NameClass> lsObject) where NameClass : class
        {
            if (lsObject != null && lsObject.Count > 0)
            {
                foreach (NameClass item in lsObject)
                {
                    dbSetObject.Attach(item);
                    dbSetObject.Remove(item);
                }
            }
        }

        protected void addObjectToDBSet_Update<NameClass>(ref ConnectionEFDataFirst context, ref DbSet<NameClass> dbSetObject, List<NameClass> lsObject) where NameClass : class
        {
            if (lsObject != null && lsObject.Count > 0)
            {
                foreach (NameClass item in lsObject)
                {
                    dbSetObject.Add(item);
                    context.Entry((object)item).State = EntityState.Modified;
                }
            }
        }

        protected void addObjectToDBSet_Insert<NameClass>(ref ConnectionEFDataFirst context, ref DbSet<NameClass> dbSetObject, List<NameClass> lsObject) where NameClass : class
        {
            if (lsObject != null && lsObject.Count > 0)
            {
                foreach (NameClass item in lsObject)
                {
                    dbSetObject.Add(item);
                }
            }
        }
        protected List<NameClass> setValueForAttributeListClassObject<NameClass>(object value, ref List<NameClass> lsObject, string nameAttribute)
        {
            if (lsObject != null && lsObject.Count > 0)
            {
                foreach (NameClass item in lsObject)
                {
                    item.GetType().GetProperty(nameAttribute).SetValue(item, value, null);
                }
            }
            return lsObject;
        }
    }
}