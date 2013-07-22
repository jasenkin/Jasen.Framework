using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jasen.Framework.Strategy
{
    /// <summary>
    /// Strategy pattern must inplement much more interface than simple factory, but is best for OCP princple ,need to replace?
    /// </summary>
    public class DatabaseStrategyContext
    {
        private DatabaseStrategy _databaseStrategy;

        public DatabaseStrategyContext(DatabaseStrategy databaseStrategy)
        {
            if (databaseStrategy == null)
            {
                throw new ArgumentNullException("databaseStrategy");
            }

            this._databaseStrategy = databaseStrategy;
        }

        internal virtual int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false)
            where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.AddNew<T>(entities, returnIdentity, commitTransaction);
        }

        internal int BetchUpdate<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.BetchUpdate<T>(entities, byIdentity, commitTransaction);
        }

        internal int Update<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.Update<T>(entities, byIdentity, commitTransaction);
        }

        internal int Update<T>(T entity, string condition, bool commitTransaction) where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.Update<T>(entity, condition, commitTransaction);
        }

        internal int Delete<T>(IList<T> entities, bool byIdentity = false,
            bool commitTransaction = false) where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.Delete<T>(entities, byIdentity, commitTransaction);
        }

        internal int Delete<T>(string condition, bool commitTransaction = false) where T : TableExecutor<T>, new()
        {
            return this._databaseStrategy.Delete<T>(condition, commitTransaction);
        }

        internal int Count(Type type, string condition)
        {
            return this._databaseStrategy.Count(type, condition);
        }

        internal double Sum(Type type, string columnName, string condition)
        {
            return this._databaseStrategy.Sum(type, columnName, condition);
        }

        internal double Max(Type type, string columnName, string condition)
        {
            return this._databaseStrategy.Max(type, columnName, condition);
        }

        internal double Min(Type type, string columnName, string condition)
        {
            return this._databaseStrategy.Min(type, columnName, condition);
        }

        internal T RetrieveById<T>(T entity, bool byIdentity = false) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.RetrieveById<T>(entity, byIdentity);
        }

        internal T RetrieveBySql<T>(string sql) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.RetrieveBySql<T>(sql);
        }

        internal bool Exists<T>(T entity, bool byIdentity = false) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.Exists<T>(entity, byIdentity);
        }

        internal bool Exists<T>(string condition) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.Exists<T>(condition);
        }

        internal DataTable QueryTop<T>(string condition, int topCount, Order order = null,
            params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.QueryTop<T>(condition, topCount, order, propertyNames);
        }

        internal DataTable Query<T>(string condition, Order order, params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.Query<T>(condition, order, propertyNames);
        }

        internal DataTable Query<T>(string condition, PagingArg pagingArg, Order order = null,
            bool sqlServer2000 = false, params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            return this._databaseStrategy.Query<T>(condition, pagingArg, order, sqlServer2000, propertyNames);
        }

        internal void RetrieveAssociation(object container, params string[] properties)
        {
            this._databaseStrategy.RetrieveAssociation(container, properties);
        }
    }
}
