using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Configuration;
using System.Data;
using Jasen.Framework.Infrastructure;
using Jasen.Framework.Reflection;
using System.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;

namespace Jasen.Framework
{
    public abstract class DbContext
    {
        private DatabaseProvider _databaseStrategy = null;

        public DbContext(string providerName, int commandTimeout = 60)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentNullException("providerName");
            }

            if (!DbProviderCache.Current.Contains(providerName.Trim()))
            {
                DbProviderCache.Current.AddProviderConfiguration(providerName.Trim(), commandTimeout);
            }

            _databaseStrategy = DbProviderCache.Current[providerName.Trim()];
        }

        public virtual int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false)
           where T : ITable, new()
        {
            return this._databaseStrategy.AddNew<T>(entities, returnIdentity, commitTransaction);
        }

        public int BetchUpdate<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : ITable, new()
        {
            return this._databaseStrategy.BetchUpdate<T>(entities, byIdentity, commitTransaction);
        }

        public int Update<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : ITable, new()
        {
            return this._databaseStrategy.Update<T>(entities, byIdentity, commitTransaction);
        }

        public int Update<T>(T entity, string condition, bool commitTransaction) where T : ITable, new()
        {
            return this._databaseStrategy.Update<T>(entity, condition, commitTransaction);
        }

        public int Delete<T>(IList<T> entities, bool byIdentity = false,
            bool commitTransaction = false) where T : ITable, new()
        {
            return this._databaseStrategy.Delete<T>(entities, byIdentity, commitTransaction);
        }

        public int Delete<T>(string condition, bool commitTransaction = false) where T : ITable, new()
        {
            return this._databaseStrategy.Delete<T>(condition, commitTransaction);
        }

        public int Count<T>(string condition) where T : IView, new()
        {
            return this._databaseStrategy.Count(typeof(T), condition);
        }

        public double Sum<T>(string columnName, string condition) where T : IView, new()
        {
            return this._databaseStrategy.Sum(typeof(T), columnName, condition);
        }

        public double Max<T>(string columnName, string condition) where T : IView, new()
        {
            return this._databaseStrategy.Max(typeof(T), columnName, condition);
        }

        public double Min<T>(string columnName, string condition) where T : IView, new()
        {
            return this._databaseStrategy.Min(typeof(T), columnName, condition);
        } 

        public T RetrieveById<T>(T entity, bool byIdentity = false) where T : IView, new()
        {
            return this._databaseStrategy.RetrieveById<T>(entity, byIdentity);
        }

        public T RetrieveBySql<T>(string sql) where T : IView, new()
        {
            return this._databaseStrategy.RetrieveBySql<T>(sql);
        }

        public bool Exists<T>(T entity, bool byIdentity = false) where T : IView, new()
        {
            return this._databaseStrategy.Exists<T>(entity, byIdentity);
        }

        public bool Exists<T>(string condition) where T : IView, new()
        {
            return this._databaseStrategy.Exists<T>(condition);
        }

        public DataTable QueryTop<T>(string condition, int topCount, Order order = null,
            params string[] propertyNames) where T : IView, new()
        {
            return this._databaseStrategy.QueryTop<T>(condition, topCount, order, propertyNames);
        }

        public DataTable Query<T>(string condition, Order order, params string[] propertyNames) where T : IView, new()
        {
            return this._databaseStrategy.Query<T>(condition, order, propertyNames);
        }

        public DataTable Query<T>(string condition, PagingArg pagingArg, Order order = null, params string[] propertyNames) where T : IView, new()
        {
            return this._databaseStrategy.Query<T>(condition, pagingArg, order, propertyNames);
        }
          
        public bool BeginTransaction()
        { 
            return this._databaseStrategy.BeginTransaction();
        }

        public bool Commit()
        { 
            return this._databaseStrategy.Commit();
        }

        public bool Rollback()
        { 
            return this._databaseStrategy.Rollback();
        }
         
        public int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters = null)
        {
            return this._databaseStrategy.ExecuteNonQuery(commandText, isProcedure, parameters);
        }

        public int ExecuteNonQuery<T>(T entity) where T : class, IStoreProcedure
        {
            return this._databaseStrategy.ExecuteNonQuery<T>(entity);
        }

        public object ExecuteScalar(string commandText, bool isProcedure, IList<IDataParameter> parameters = null)
        {
            return this._databaseStrategy.ExecuteScalar(commandText, isProcedure, parameters);
        }

        public object ExecuteScalar<T>(T entity) where T : class, IStoreProcedure
        {
            return this._databaseStrategy.ExecuteScalar<T>(entity);
        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure, IList<IDataParameter> parameters = null)
        {
            return this._databaseStrategy.ExecuteReader(commandText, isProcedure, parameters);
        }


        public IDataReader ExecuteReader<T>(T entity) where T : class, IStoreProcedure
        {
            return this._databaseStrategy.ExecuteReader<T>(entity);
        }

        public void CloseReader(IDataReader reader)
        {
             this._databaseStrategy.CloseReader(reader);
        }

        public DataTable ExecuteDataTable(string commandText, bool isProcedure , IList<IDataParameter> parameters = null)
        {
            return this._databaseStrategy.ExecuteDataTable(commandText, isProcedure, parameters);
        }

        public DataTable ExecuteDataTable<T>(T entity) where T : class, IStoreProcedure
        {
            return this._databaseStrategy.ExecuteDataTable<T>(entity);
        } 

        public DataSet ExecuteDataSet(string commandText, bool isProcedure, IList<IDataParameter> parameters = null)
        {
            return this._databaseStrategy.ExecuteDataSet(commandText, isProcedure, parameters);
        }

        public DataSet ExecuteDataSet<T>(T entity) where T : class, IStoreProcedure
        {
            return this._databaseStrategy.ExecuteDataSet<T>(entity);
        }
    }
}
