using System;
using System.Collections.Generic;
using System.Data;
using Jasen.Framework.Attributes;
using Jasen.Framework.Core; 

namespace Jasen.Framework
{
    public interface ISqlBuilder
    {
        IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute,
                                                string parameterName, object parameterValue);
      
        IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute,
                                             string parameterName, object parameterValue);

        string CreateOneToOneAssociationSql(AssociationAttribute currentAttribute,
                                               Type elementType, object container);

        string CreateOneToManyAssociationSql(AssociationAttribute currentAttribute,
                                             Type elementType, object container);

        string CreateManyToOneAssociationSql(AssociationAttribute currentAttribute,
                                                Type elementType, object container);

        string CreateManyToManyAssociationSql(AssociationAttribute currentAttribute,
                                                 Type elementType, object container);

        SqlCommandParameter CreateSqlInsertCommandParameter<T>(T entity, IList<string> propertyNames,
            IdentityPrimaryKeyInfo keyInfo, bool returnIdentity = true)
            where T : TableExecutor<T>, new();

        SqlCommandParameter CreateSqlUpdateCommandParameter<T>(T entity, IdentityPrimaryKeyInfo keyInfo,
            bool byIdentity = true) where T : TableExecutor<T>, new();
        SqlCommandParameter CreateSqlUpdateCommandParameter<T>(T entity, string condition) where T : TableExecutor<T>, new();

        string Delete<T>(string condition) where T : TableExecutor<T>, new();
        string DeleteById<T>(T entity) where T : TableExecutor<T>, new();
        string DeleteByPK<T>(T entity) where T : TableExecutor<T>, new();

        string Exists(Type entityType, string condition);
        string ExistsById(object entity);
        string ExistsByPK(object entity);

        string SelectByCondition(Type entityType, string condition, Order order = null, IList<string> propertyNames = null); 
        string SelectById(object entity);
        string SelectByPK(object entity);
        string SelectNextIdentity(Type entityType);
        string SelectCurrentIdentity(Type entityType);
        string SelectPaging(Type entityType, string condition, int pageSize, int pageIndex, Order order, IList<string> propertyNames = null);
        string SelectTop(Type entityType, string condition, int topCount, Order order = null, IList<string> propertyNames = null);
        
        string Count(Type entityType, string condition);
        string Sum(Type entityType, string columnName, string condition);
        string Max(Type entityType, string columnName, string condition);
        string Min(Type entityType, string columnName, string condition);
         
    }
}
