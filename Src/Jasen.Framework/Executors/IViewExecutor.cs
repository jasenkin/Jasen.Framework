using System; 
using System.Collections.Generic;
using System.Data; 

namespace Jasen.Framework
{
    public interface IViewExecutor<T> where T : IViewExecutor<T>, new()
    { 
        int Count();
        int Count(string condition);
        bool Exists(string condition);

        void RetrieveAssociations(params string[] propertyNames);

        IList<T> Find(PagingArg pagingArg, Order order = null, bool sqlServer2000 = false, params string[] propertyNames);
        IList<T> Find(string condition, Order order, params string[] propertyNames);
        IList<T> Find(string condition, PagingArg pagingArg, Order order = null, bool sqlServer2000 = false, params string[] propertyNames);
        IList<T> Find(string condition, params string[] propertyNames);
        IList<T> FindAll(params string[] propertyNames);
        IList<T> FindTop(int topCount, Order order = null, params string[] propertyNames);
        IList<T> FindTop(string condition, int topCount, Order order = null, params string[] propertyNames);
        object Max(string columnName);
        object Max(string columnName, string condition);
        object Min(string columnName);
        object Min(string columnName, string condition);
        DataTable Query(PagingArg pagingArg, Order order = null, bool sqlServer2000 = false, params string[] propertyNames);
        DataTable Query(string condition, Order order, params string[] propertyNames);
        DataTable Query(string condition, PagingArg pagingArg, Order order = null, bool sqlServer2000 = false, params string[] propertyNames);
        DataTable Query(string condition, params string[] propertyNames);
        DataTable QueryAll(params string[] propertyNames);
        DataTable QueryTop(int topCount, Order order = null, params string[] propertyNames);
        DataTable QueryTop(string condition, int topCount, Order order = null, params string[] propertyNames);

        bool ExistsById(T entity);
        bool ExistsByPK(T entity);

        T RetrieveById(T entity);
        T RetrieveByPK(T entity);
        T RetrieveBySql(string sql);
        double Sum(string columnName);
        double Sum(string columnName, string condition);
        IList<T> ToEntities(DataTable adaptedTable);
        T ToEntity(DataRow adaptedRow);
        DataTable ToTable(IList<T> entities, bool isAdapted = false);
    }
}
