using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Jasen.Framework.Configuration;
using Jasen.Framework.Strategy; 

namespace Jasen.Framework
{
    public class ViewExecutor<T> : IViewExecutor<T> where T : ViewExecutor<T>, new()
    {   
        private DatabaseStrategy _strategy;
      
        protected DatabaseStrategy DatabaseStrategy
        {
            get
            {
                if (this._strategy == null)
                {
                    this._strategy = DatabaseStrategyFactory.CreateStrategy(this.Config);
                }
                return this._strategy;
            }  
        }

        protected DatabaseConfig Config
        {
            get
            {
                return ConfigManager.Current.FindConfig(typeof(T));
            }
        }

        protected T Entity { get; set; }

        public ViewExecutor()
        {
            this.Entity = (T)this;
        }

        public void RetrieveAssociations(params string[] propertyName)
        {
            this.DatabaseStrategy.RetrieveAssociation(this, propertyName);
        }


        #region Common

        public int Count(string condition)
        {
            return this.DatabaseStrategy.Count(typeof(T), condition);
           
        }

        public int Count()
        {
            return Count(" 1=1 ");
        }

        public double Sum(string columnName)
        {
            return Sum(columnName, " 1=1 ");
        }

        public double Sum(string columnName, string condition)
        {
            return this.DatabaseStrategy.Sum(typeof(T),columnName, condition); 
        }

        public object Max(string columnName, string condition)
        {
            return this.DatabaseStrategy.Max(typeof(T), columnName, condition); 
        }

        public  object Max(string columnName)
        {
            return Max(columnName, " 1=1 ");
        }
  
        public  object Min(string columnName, string condition)
        {
            return this.DatabaseStrategy.Min(typeof(T), columnName, condition); 
        }

        public  object Min(string columnName)
        {
            return Min(columnName, " 1=1 ");
        }
         
        #endregion  

        #region  ID PK

        public T RetrieveById(T entity)
        {
            return this.DatabaseStrategy.RetrieveById<T>(entity, true); 
        }

        public T RetrieveByPK(T entity)
        {
            return this.DatabaseStrategy.RetrieveById<T>(entity, false); 
        }

        public T RetrieveBySql(string sql)
        {
            return this.DatabaseStrategy.RetrieveBySql<T>(sql); 
        }
 
        public bool ExistsById(T entity)
        {
            return this.DatabaseStrategy.Exists<T>(entity, true);
        }

        public bool ExistsByPK(T entity)
        {
            return this.DatabaseStrategy.Exists<T>(entity, true);
        }

        public  bool Exists(string condition)
        {
            return this.DatabaseStrategy.Exists<T>(condition);
        }
         
        #endregion   

        #region Find And Query

        public IList<T> FindTop(string condition, int topCount, Order order = null, params string[] propertyNames)
        {
            DataTable dt = QueryTop(condition, topCount, order, propertyNames);
            return EntityMapper<T>.ToEntities(dt);
        }

        public IList<T> FindTop(int topCount, Order order = null, params string[] propertyNames)
        {
            return FindTop("1=1", topCount, order, propertyNames);
        }

        public DataTable QueryTop(int topCount, Order order = null, params string[] propertyNames)
        {
            return QueryTop("1=1", topCount, order, propertyNames);
        }

        public DataTable QueryTop(string condition, int topCount, Order order = null, params string[] propertyNames)
        {
            return this.DatabaseStrategy.QueryTop<T>(condition, topCount, order, propertyNames);
        }

        public IList<T> Find(string condition, params string[] propertyNames)
        {
            return Find(condition, Order.Empty, propertyNames);
        }

        public IList<T> Find(string condition, Order order, params string[] propertyNames)
        {
            DataTable dt = Query(condition, order, propertyNames);
            return EntityMapper<T>.ToEntities(dt);
        }

        public IList<T> FindAll(params string[] propertyNames)
        {
            DataTable dt = QueryAll(propertyNames);
            return EntityMapper<T>.ToEntities(dt);
        }

        public DataTable Query(string condition, params string[] propertyNames)
        {
            return Query(condition, Order.Empty, propertyNames);
        }

        public DataTable Query(string condition, Order order, params string[] propertyNames)
        {
            return this.DatabaseStrategy.Query<T>(condition, order, propertyNames);
        }

        public DataTable QueryAll(params string[] propertyNames)
        {
            return Query(" 1=1 ", propertyNames);
        }

        public IList<T> Find(string condition, PagingArg pagingArg, Order order = null,
            bool sqlServer2000 = false, params string[] propertyNames)
        {
            DataTable dt = Query(condition, pagingArg, order, sqlServer2000, propertyNames);
            return EntityMapper<T>.ToEntities(dt);
        }

        public IList<T> Find(PagingArg pagingArg, Order order = null, bool sqlServer2000 = false,
            params string[] propertyNames)
        {
            return Find(" 1=1 ", pagingArg, order, sqlServer2000, propertyNames);
        }

        public DataTable Query(string condition, PagingArg pagingArg, Order order = null,
            bool sqlServer2000 = false, params string[] propertyNames)
        {
            return this.DatabaseStrategy.Query<T>(condition, pagingArg, order, sqlServer2000, propertyNames);
        }

        public DataTable Query(PagingArg pagingArg, Order order = null, bool sqlServer2000 = false,
            params string[] propertyNames)
        {
            return Query(" 1=1 ", pagingArg, order, sqlServer2000, propertyNames);
        } 
        #endregion  

        #region  Convert

        public T ToEntity(DataRow adaptedRow)
        {
            return EntityMapper<T>.ToEntity(adaptedRow);
        }

        public IList<T> ToEntities(DataTable adaptedTable)
        {
            return EntityMapper<T>.ToEntities(adaptedTable);
        }

        public ArrayList ToEntityArrayList(DataTable adaptedTable)
        {
            var entities = EntityMapper<T>.ToEntities(adaptedTable);

            if(entities==null||entities.Count==0)
            {
                return new ArrayList();
            }

            var arrayList = new ArrayList();
            foreach(var entity in entities)
            {
                arrayList.Add(entity);
            }

            return arrayList;
        }

        public T[] ToEntityArray(DataTable adaptedTable)
        {
            return EntityMapper<T>.ToEntities(adaptedTable).ToArray();
        }

        public DataTable ToTable(IList<T> entities, bool isAdapted = false)
        {
            return EntityMapper<T>.ToTable(entities, isAdapted);
        }
     
        #endregion  
    }
}
