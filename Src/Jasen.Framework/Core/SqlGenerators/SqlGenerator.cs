using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Jasen.Framework.Attributes;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;
using Jasen.Framework.Reflection;
using Jasen.Framework.Infrastructure;

namespace Jasen.Framework
{ 
    public abstract class SqlGenerator : ISqlGenerator
    {
        public const string SQL_PARAMETER_PREFIX_SIGN = "@";
        public const string ORACLE_PARAMETER_PREFIX_SIGN = ":";
        public const string DEFAULT_PARAMETER_PREFIX_SIGN = "";

        public abstract IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute,
                                                                string parameterName, object parameterValue);
      
        public string ConvertAndFormatColumnNames(Type entityType, IList<string> propertyNames)
        {
            var columnNames = GetColumnNames(entityType, propertyNames);

            return FormatColumnNames(columnNames);
        }

        public IList<string> GetColumnNames(Type entityType, IList<string> propertyNames)
        {
            IList<string> columnNames = new List<string>();

            if (propertyNames == null || propertyNames.Count <= 0)
            {
                columnNames = GetAllColumnNameList(entityType);
            }
            else
            {
                columnNames = ConvertPropertiesToColumnNames(entityType, propertyNames);
            }

            return columnNames;
        }

        public static IList<string> GetAllColumnNameList(Type entityType)
        {
            var columnNames = new List<string>();

            if (entityType == null || entityType.GetProperties().Length <= 0)
            {
                return columnNames;
            }

            string columnName;
            ColumnAttribute columnAttribute;

            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute != null)
                {
                    columnName = string.IsNullOrEmpty(columnAttribute.ColumnName) ? propertyInfo.Name : columnAttribute.ColumnName;
                    columnNames.Add(columnName);
                }
            }

            return columnNames;
        }

        public static List<string> ConvertPropertiesToColumnNames(Type entityType,
           IEnumerable<string> propertyNames, bool isInsertSql = false)
        {
            var columnNames = new List<string>();

            if (entityType == null || entityType.GetProperties().Length <= 0)
            {
                return columnNames;
            }

            string columnName;
            ColumnAttribute columnAttribute = null;

            foreach (string propertyName in propertyNames)
            {
                var propertyInfo = entityType.GetProperty(propertyName);

                if (propertyInfo == null)
                {
                    continue;
                }

                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute != null)
                { 
                    if (isInsertSql && columnAttribute.IsIdentity)
                    {
                        continue;
                    }

                    columnName = string.IsNullOrEmpty(columnAttribute.ColumnName) ? propertyInfo.Name : columnAttribute.ColumnName;
                    columnNames.Add(columnName);
                }
            }

            return columnNames;
        }

        public static string FormatColumnNames(IList<string> columnNames)
        {
            if (columnNames == null || columnNames.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(",", columnNames);
        }

        public virtual string FormatParameterList(IList<string> columnNames)
        {
            return this.FormatParameterList(columnNames, SQL_PARAMETER_PREFIX_SIGN);
        }

        public string FormatParameterList(IList<string> columnNames, string prefix)
        {
            if (columnNames == null || columnNames.Count == 0)
            {
                return string.Empty;
            }

            for (int index = 0; index < columnNames.Count; index++)
            {
                columnNames[index] = prefix + columnNames[index];    
            }

            return FormatColumnNames(columnNames);
        }

        public static string FormatColumnAndParameterPairs(IList<string> columnNames, string prefix = SQL_PARAMETER_PREFIX_SIGN)
        {
            if (columnNames == null || columnNames.Count == 0)
            {
                return string.Empty;
            }

            for (int index = 0; index < columnNames.Count; index++)
            {
                columnNames[index] = columnNames[index] + "=" + prefix + columnNames[index];            
            }

            return FormatColumnNames(columnNames);
        }   

        private static string BuildExpressionById(object entity)
        {
            string identityName;
            object identityValue;
            IdentityUtility.GetIdentity(entity, out identityName, out identityValue);

            return BuildExpression(identityName, identityValue);
        }

        private static string BuildExpression(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(MsgResource.IdentityOfEntityMissing);
            }

            return key + " = " + BuildValueExpression(value);
        }

        private static string BuildValueExpression(object columnValue)
        {
            if (columnValue.GetType() == typeof(string))
            {
                return "'" + columnValue + "'";
            }
            else if (columnValue.GetType() == typeof(DateTime))
            {
                DateTime dateTime = DateTime.Parse(columnValue.ToString(), CultureInfo.CurrentCulture);
                return "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (columnValue.GetType() == typeof(bool))
            {
                int flag = bool.Parse(columnValue.ToString()) ? 1 : 0;
                return flag.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                return columnValue.ToString();
            }
        }

        private static string BuildExpressionByPrimaryKey(object entity)
        {
            string primaryKey;
            object primaryKeyValue;
            KeyUtility.GetPrimaryKey(entity, out primaryKey, out primaryKeyValue);

            return BuildExpression(primaryKey, primaryKeyValue); 
        }
         
        public string CreateOneToOneAssociationSql(AssociationAttribute attribute,
            Type elementType, object container)
        {  
            var containerPrimaryKey = KeyUtility.GetPrimaryKeyProperty(container.GetType());

            if (containerPrimaryKey == null)
            {
                throw new ArgumentException(MsgResource.ParentEntityMustContainPrimaryKey);
            }

            string elementPrimaryKey = KeyUtility.GetPrimaryKeyName(elementType);

            if (string.IsNullOrEmpty(elementPrimaryKey))
            {
                throw new ArgumentException(MsgResource.EntityMustContainPrimaryKey);
            }

            object primaryKeyValue = containerPrimaryKey.GetValue(container, null); 
            string condition = BuildExpression(elementPrimaryKey, primaryKeyValue); 

            if (attribute.ResultFilter != null)
            {
                condition += " AND " + attribute.ResultFilter;
            }

            return this.SelectByCondition(elementType, condition, null);
        }

        public string CreateOneToManyAssociationSql(AssociationAttribute attribute,
           Type elementType, object container)
        { 
            var containerPrimaryKey = KeyUtility.GetPrimaryKeyProperty(container.GetType());

            if (containerPrimaryKey == null)
            {
                throw new ArgumentException(MsgResource.ParentEntityMustContainPrimaryKey);
            }

            string elementForeignKey = KeyUtility.GetForeignKey(elementType, container.GetType());

            if (string.IsNullOrEmpty(elementForeignKey))
            {
                throw new ArgumentException("Child Entity Must Contain ForeignKey ");
            }

            object containerPrimaryKeyValue = containerPrimaryKey.GetValue(container, null);
            string condition = BuildExpression(elementForeignKey, containerPrimaryKeyValue); 

            if (!string.IsNullOrEmpty(attribute.ResultFilter))
            {
                condition = condition + " AND " + attribute.ResultFilter;
            }

            return this.SelectByCondition(elementType, condition, attribute.ResultOrder);
        }

        public string CreateManyToOneAssociationSql(AssociationAttribute currentAttribute,
           Type elementType, object container)
        {
            string elementPrimaryKey = KeyUtility.GetPrimaryKeyName(elementType);
 
            if (string.IsNullOrEmpty(elementPrimaryKey))
            {
                throw new ArgumentNullException(MsgResource.EntityMustContainPrimaryKey);
            }

            var containerForeignKey = KeyUtility.GetForeignKeyProperty(container.GetType(), elementType);

            if(containerForeignKey==null)
            {
                throw new ArgumentException("Entity must have one ForeignKey");
            }

            var containerForeignKeyValue = containerForeignKey.GetValue(container, null);

            string condition = BuildExpression(elementPrimaryKey, containerForeignKeyValue);

            if (!string.IsNullOrEmpty(currentAttribute.ResultFilter))
            {
                condition = condition + " AND " + currentAttribute.ResultFilter;
            }

            return this.SelectByCondition(elementType, condition);
        }

        public string CreateManyToManyAssociationSql(AssociationAttribute attribute,
           Type elementType, object container)
        {
            var containerPrimaryKey = KeyUtility.GetPrimaryKeyProperty(container.GetType());
            if (containerPrimaryKey == null)
            {
                throw new ArgumentException(MsgResource.ParentEntityMustContainPrimaryKey);
            }

            if(attribute.InterrelationType==null)
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            string interrelForeignKey = KeyUtility.GetForeignKey(attribute.InterrelationType, container.GetType());

            if (string.IsNullOrWhiteSpace(interrelForeignKey))
            {
                throw new ArgumentException(MsgResource.ParentEntityMustContainPrimaryKey);
            }

            object containerPrimaryKeyValue = containerPrimaryKey.GetValue(container, null);
            string condition = BuildExpression(interrelForeignKey, containerPrimaryKeyValue);

            if (!string.IsNullOrWhiteSpace(attribute.InterrelationFilter))
            {
                condition = condition + " AND " + attribute.InterrelationFilter;
            }

            if (string.IsNullOrWhiteSpace(condition))
            {
                condition = " null=null ";
            }

            string elementPrimaryKey = KeyUtility.GetPrimaryKeyName(elementType);

            if(string.IsNullOrWhiteSpace(elementPrimaryKey))
            {
                throw new ArgumentException(MsgResource.EntityMustContainPrimaryKey);
            }

            StringBuilder sql =new StringBuilder();
            sql.Append("SELECT ");
            sql.Append(ConvertAndFormatColumnNames(elementType, null));
            sql.Append(" FROM ");
            sql.Append(AttributeUtility.GetTableName(elementType));
            sql.Append(" WHERE " + elementPrimaryKey);

            string foreignKey = KeyUtility.GetForeignKey(attribute.InterrelationType, elementType);

            if (string.IsNullOrWhiteSpace(foreignKey))
            {
                throw new ArgumentException(MsgResource.RelationAttributeShouldHasAForeignKey);
            }

            sql.Append(" IN ( SELECT " + foreignKey + " FROM " + AttributeUtility.GetTableName(attribute.InterrelationType));
            sql.Append(" WHERE " + condition + " )");
 
            if (attribute.ResultOrder != null && !string.IsNullOrEmpty(attribute.ResultOrder.OrderString))
            {
                sql.Append(" ORDER BY " + attribute.ResultOrder.OrderString);
            }

            return sql.ToString();
        }

        public string Delete<T>(string condition) where T : ITable, new()
        {
            string tableName = CheckAndGetTableName(typeof(T));

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }
 
            return string.Concat("DELETE  FROM ", tableName, " WHERE ", condition);
        }

        public string DeleteById<T>(T entity) where T : ITable, new()
        { 
            return Delete<T>(BuildExpressionById(entity));
        }

        public string DeleteByPK<T>(T entity) where T : ITable, new()
        {
            return Delete<T>(BuildExpressionByPrimaryKey(entity));
        }

        public abstract string Exists(Type entityType, string condition);

        public string ExistsById(object entity)
        { 
            return this.Exists(entity.GetType(), BuildExpressionById(entity));
        }

        public string ExistsByPK(object entity)
        { 
            return this.Exists(entity.GetType(), BuildExpressionByPrimaryKey(entity));
        }

        public string Count(Type entityType, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT COUNT(*) FROM ", tableName, " WHERE ", condition);
        }

        public string Sum(Type entityType, string columnName, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT SUM(", columnName, ") FROM ", tableName, " WHERE ", condition);
        }

        public string Max(Type entityType, string columnName, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT MAX(", columnName, ") FROM ", tableName, " WHERE ", condition);
        }

        public string Min(Type entityType, string columnName, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT MIN(", columnName, ") FROM ", tableName, " WHERE ", condition);
        }

        public SqlCommandParameter CreateSqlUpdateCommandParameter<T>(T entity, IdentityPrimaryKeyInfo keyInfo,
            bool byIdentity = true) where T : ITable, new()
        {
            if (entity == null || keyInfo == null)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments);
            }
             
            object keyValue = keyInfo.PropertyInfo.GetValue(entity, null);
            string condition = BuildExpression(keyInfo.Name ,keyValue);

            return CreateUpdateCommand<T>(entity, byIdentity, condition);
        }

        private SqlCommandParameter CreateUpdateCommand<T>(T entity, bool? byIdentity, string condition) 
            where T : ITable, new()
        {
            List<string> propertyNames = entity.ChangedPropertyNames;

            if (propertyNames == null || propertyNames.Count <= 0)
            {
                throw new ArgumentException(MsgResource.InvalidArguments + "propertyNames");
            }

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            string tableName = CheckAndGetTableName(typeof(T));
            var parameter = new SqlCommandParameter();
            IList<string> columnNames = new List<string>();
            string columnName = null;
            object columnValue = null;
            IDataParameter dataParameter = null;
            ColumnAttribute columnAttribute = null;

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);

                if (propertyInfo == null)
                {
                    continue;
                }

                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute != null)
                {
                    if (columnAttribute.IsIdentity)
                    {
                        continue;
                    }

                    if (columnAttribute.IsPrimaryKey && byIdentity.HasValue && !byIdentity.Value)
                    {
                        continue;
                    }

                    columnValue = propertyInfo.GetValue(entity, null) ?? DBNull.Value;
                    columnName = string.IsNullOrEmpty(columnAttribute.ColumnName)
                                     ? propertyInfo.Name
                                     : columnAttribute.ColumnName;

                    dataParameter = CreateColumnParameter(columnAttribute, columnName, columnValue);
                    parameter.Parameters.Add(dataParameter);

                    columnNames.Add(columnName);
                }
            }

            parameter.Sql = string.Concat("UPDATE ", tableName, " SET " + FormatColumnAndParameterPairs(columnNames),
                " WHERE ", condition);


            return parameter;
        }

        public SqlCommandParameter CreateSqlUpdateCommandParameter<T>(T entity, string condition) where T : ITable, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments);
            }

            return CreateUpdateCommand<T>(entity, null, condition);
        }
 
        public abstract IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute, string columnName, object columnValue);
 
        protected abstract string FormatColumnAndParameterPairs(IList<string> columnNames);
  
        public string SelectByCondition(Type entityType, string condition,
            Order order = null, IList<string> propertyNames = null)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            var sql = new StringBuilder();

            sql.Append("SELECT ");

            IList<string> columnNames = GetColumnNames(entityType, propertyNames);

            if(columnNames==null||columnNames.Count==0)
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            sql.Append(FormatColumnNames(columnNames));

            sql.Append(" FROM " + tableName);
            sql.Append(" WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            return sql.ToString();
        }

        public string SelectById(object entity)
        { 
            return this.SelectByCondition(entity.GetType(), BuildExpressionById(entity));
        }

        public string SelectByPK(object entity)
        { 
            return this.SelectByCondition(entity.GetType(), BuildExpressionByPrimaryKey(entity));
        }

        public virtual SqlCommandParameter CreateSqlInsertCommandParameter<T>(T entity, IList<string> propertyNames,
            IdentityPrimaryKeyInfo identityInfo, bool returnIdentity = true) where T : ITable, new()
        {
            if (propertyNames == null || propertyNames.Count <= 0)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments + "propertyNames");
            }

            string tableName = CheckAndGetTableName(entity.GetType());

            var parameter = new SqlCommandParameter();
            string columnName;
            object columnValue;
            IDataParameter dataParameter;
            ColumnAttribute columnAttribute;
            var columnNames = new List<string>();

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName);

                if (propertyInfo == null)
                {
                    continue;
                }

                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute == null || columnAttribute.IsIdentity)
                {
                    continue;
                }

                columnValue = propertyInfo.GetValue(entity, null) ?? DBNull.Value;
                columnName = string.IsNullOrEmpty(columnAttribute.ColumnName)
                                 ? propertyInfo.Name
                                 : columnAttribute.ColumnName;

                dataParameter = this.CreateColumnParameter(columnAttribute, columnName, columnValue);

                columnNames.Add(columnName);
                parameter.Parameters.Add(dataParameter);
            }

            string columnNameList = FormatColumnNames(columnNames);
            string parameterList = FormatParameterList(columnNames);

            if (string.IsNullOrEmpty(columnNameList) || string.IsNullOrEmpty(parameterList))
            {
                throw new ArgumentNullException(MsgResource.InvalidEntityConfig);
            }

            parameter.Sql = "INSERT INTO " + tableName + "(" + columnNameList +
                ")VALUES(" + parameterList + ");";

            return parameter;
        }

        public abstract string SelectNextIdentity(Type entityType);

        public abstract string SelectCurrentIdentity(Type entityType);

        public abstract string SelectPaging(Type entityType, string condition,
            int pageSize, int pageIndex, Order order, IList<string> propertyNames = null);

        public abstract string SelectTop(Type entityType, string condition,
            int topCount, Order order = null, IList<string> propertyNames = null);

        protected string CheckAndGetTableName(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(MsgResource.EmptyEntity);
            }

            string tableName = AttributeUtility.GetTableName(entityType);

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(MsgResource.InvalidEntityConfig);
            }

            return tableName;
        }

       
    }
}
