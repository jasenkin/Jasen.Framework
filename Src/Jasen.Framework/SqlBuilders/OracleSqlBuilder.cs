using System;
using System.Data;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Globalization;
using System.Text;

using Jasen.Framework;
using Jasen.Framework.Resources;
using System.Reflection;
using Jasen.Framework.Core;
using Jasen.Framework.Reflection;

namespace Jasen.Framework
{ 
    internal class OracleSqlBuilder : SqlBuilder
    {
        public override IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute,
            string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, columnAttribute.OracleType);
        }

        public override IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute, string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, parameterAttribute.OracleType);
        }

        private static IDataParameter CreateParameter(string parameterName, object parameterValue, OracleType dbType)
        {
            parameterValue = parameterValue ?? DBNull.Value;

            if (parameterValue is DBNull)
            {
                return new OracleParameter(parameterName, dbType);
            }

            return new OracleParameter(parameterName, parameterValue);
        }
       
        public override string FormatParameterList(IList<string> columnNames)
        {
            return base.FormatParameterList(columnNames, DatabaseType.Oracle);
        }

        public override string Exists(Type entityType, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT CASE WHEN EXISTS(SELECT * FROM ", tableName,
            " WHERE ", condition, ")THEN 1 ELSE 0 END CASE FROM DUAL ");
        }

        public override string SelectNextIdentity(Type entityType)
        {
            TableAttribute tableAttribute = AttributeUtility.GetTableAttribute(entityType);

            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.TableName))
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            return SelectNextIdentity( tableAttribute.IdSequence);
        }

        public override string SelectCurrentIdentity(Type entityType)
        {
            TableAttribute tableAttribute = AttributeUtility.GetTableAttribute(entityType);

            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.TableName))
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            return string.Concat("SELECT ", tableAttribute.IdSequence.Trim(), ".CURRVAL  FROM DUAL");
        }

        private string SelectNextIdentity(string idSequence)
        {
            if (string.IsNullOrWhiteSpace(idSequence))
            {
                throw new ArgumentNullException("idSequence");
            }

            return string.Concat("SELECT ", idSequence.Trim(), ".NEXTVAL FROM DUAL");
        }

        public override SqlCommandParameter CreateSqlInsertCommandParameter<T>(T entity,
           IList<string> propertyNames, IdentityPrimaryKeyInfo identityInfo, bool returnIdentity = true)
        {
            if (propertyNames == null || propertyNames.Count <= 0)
            {
                throw new ArgumentNullException(MsgResource.InvalidArguments + "propertyNames");
            }

            TableAttribute tableAttribute = AttributeUtility.GetTableAttribute(typeof(T));
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

                if (columnAttribute == null)
                {
                    continue;
                }

                if (columnAttribute.IsIdentity)
                {
                    if (string.IsNullOrWhiteSpace(tableAttribute.IdSequence))
                    {
                        throw new ArgumentException("If Oracle has Identity,it must have IdSequence.");
                    }
                }

                columnName = string.IsNullOrEmpty(columnAttribute.ColumnName)
                              ? propertyInfo.Name
                              : columnAttribute.ColumnName;
                columnValue = propertyInfo.GetValue(entity, null) ?? DBNull.Value;
                dataParameter = this.CreateColumnParameter(columnAttribute, columnName, columnValue);
                columnNames.Add(columnName);

                parameter.Parameters.Add(dataParameter);
            }

            if (!string.IsNullOrWhiteSpace(tableAttribute.IdSequence) && identityInfo != null) ;
            {
                columnNames.Add(identityInfo.Name);
            }

            string columnNameList = FormatColumnNames(columnNames);
            string parameterList = FormatParameterList(columnNames);

            if (string.IsNullOrEmpty(columnNameList) || string.IsNullOrEmpty(parameterList))
            {
                throw new ArgumentNullException(MsgResource.InvalidEntityConfig);
            }

            parameter.Sql = string.Concat("INSERT INTO ", tableAttribute.TableName, "(", columnNameList, ")VALUES(", parameterList, ")");

            return parameter;
        }

        public override string SelectTop(Type entityType, string condition, int topCount,
            Order order = null, IList<string> propertyNames = null)
        {
            if (topCount <= 0)
            {
                throw new ArgumentOutOfRangeException("topCount");
            }

            if (order != null && (string.IsNullOrEmpty(order.OrderColumn) || string.IsNullOrEmpty(order.OrderString)))
            {
                throw new ArgumentNullException(MsgResource.SortedOrderRequired);
            }

            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }
 
            StringBuilder sql = new StringBuilder();
            
            sql.Append("SELECT ");
            sql.Append(ConvertAndFormatColumnNames(entityType, propertyNames));
            sql.Append(" FROM " + tableName);
            sql.Append(" WHERE " + condition);

            if (order != null)
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            return topCount > 0 ? "SELECT * FROM (" + sql.ToString() + ") WHERE RowNum<" + (topCount + 1) : sql.ToString();
        }

        public override string SelectPaging(Type entityType, string condition,
            int pageSize, int pageIndex, Order order, IList<string> propertyNames = null)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            string tableName = CheckAndGetTableName(entityType);

            if (order != null && (string.IsNullOrEmpty(order.OrderColumn) || string.IsNullOrEmpty(order.OrderString)))
            {
                throw new ArgumentNullException(MsgResource.SortedOrderRequired);
            }

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            string columnNameList = ConvertAndFormatColumnNames(entityType, propertyNames);

            pageIndex = pageIndex < 0 ? 0 : pageIndex;

            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT * FROM( SELECT ROWNUM RowNumber," + columnNameList);
            builder.Append(" FROM(SELECT " + columnNameList + " FROM " + tableName + " WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                builder.Append(" ORDER BY " + order.OrderString);
            }

            builder.Append("))WHERE RowNumber <=" + (pageIndex + 1) * pageSize + " AND RowNumber >" + pageIndex * pageSize);

            return builder.ToString();
        }
 
        protected override string FormatColumnAndParameterPairs(IList<string> columnNames)
        {
            return FormatColumnAndParameterPairs(columnNames, DatabaseType.Oracle);
        }
         
    }
}
