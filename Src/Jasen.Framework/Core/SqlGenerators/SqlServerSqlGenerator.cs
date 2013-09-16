using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Text;
using Jasen.Framework.Resources; 
using Jasen.Framework.Core;
using Jasen.Framework.Reflection;

namespace Jasen.Framework
{

    public class SqlServerSqlGenerator : SqlGenerator
    {
        public override IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute,
            string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, columnAttribute.SqlDbType);
        }

        public override IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute,
            string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, parameterAttribute.SqlDbType);
        }

        private static IDataParameter CreateParameter(string parameterName, object parameterValue, SqlDbType dbType)
        {
            parameterValue = parameterValue ?? DBNull.Value;

            if (parameterValue is DBNull)
            {
                return new SqlParameter(parameterName, dbType);
            }

            return new SqlParameter(parameterName, parameterValue);
        }

        public override string FormatParameterList(IList<string> columnNames)
        {
            return base.FormatParameterList(columnNames, SQL_PARAMETER_PREFIX_SIGN);
        }
         
        public override string Exists(Type entityType, string condition)
        {
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            return string.Concat("SELECT CASE WHEN EXISTS(SELECT * FROM ", tableName,
                           " WHERE " , condition,") THEN 1 ELSE 0 END");
        }

        public override string SelectNextIdentity(Type entityType)
        {
            string tableName = CheckAndGetTableName(entityType);

            string identityName = IdentityUtility.GetIdentityName(entityType);

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(MsgResource.IdentityOfEntityMissing);
            } 

            return string.Concat("SELECT MAX(", identityName, ")+1 FROM ", tableName);
        }

        public override string SelectCurrentIdentity(Type entityType)
        {
            return string.Concat(" SELECT @@IDENTITY ");
        }

        public override SqlCommandParameter CreateSqlInsertCommandParameter<T>(T entity,
            IList<string> propertyNames, IdentityPrimaryKeyInfo identityInfo, bool returnIdentity = true)
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
                
                columnName = string.IsNullOrWhiteSpace(columnAttribute.ColumnName)
                                 ? propertyInfo.Name.Trim()
                                 : columnAttribute.ColumnName.Trim();
                columnValue = propertyInfo.GetValue(entity, null) ?? DBNull.Value;
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
 
             if (returnIdentity && identityInfo != null)
            {
                parameter.Sql += " SET " + "@" + identityInfo.Name + "=@@IDENTITY";

                object identityValue = identityInfo.PropertyInfo.GetValue(entity, null); 
                var identityParameter = this.CreateColumnParameter(identityInfo.ColumnAttribute,
                    identityInfo.Name, identityValue);
                identityParameter.Direction = ParameterDirection.Output;

                parameter.Parameters.Add(identityParameter); 
            }

            return parameter;
        }

        protected override string FormatColumnAndParameterPairs(IList<string> columnNames)
        {
            return FormatColumnAndParameterPairs(columnNames, SQL_PARAMETER_PREFIX_SIGN);
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

            if (pageIndex == 0)
            {
                return "SELECT TOP " + pageSize + " " + columnNameList + " FROM " + tableName +
                      " WHERE " + condition + " ORDER BY " + order.OrderString;
            }

            string statement = "SELECT RowNumber," + columnNameList +
                " FROM (SELECT " + columnNameList + ",ROW_NUMBER() OVER (ORDER BY " + order.OrderString + ") AS RowNumber FROM " + tableName;
            statement += " WHERE " + condition;
            statement += " ) AS T WHERE T.RowNumber > (" + pageSize * pageIndex + ")  AND T.RowNumber <= (" + (pageIndex + 1) * pageSize + ")";

            return statement;
        }
 
        public override string SelectTop(Type entityType, string condition, int topCount,
            Order order = null, IList<string> propertyNames = null)
        {
            if (topCount <= 0)
            {
                throw new ArgumentOutOfRangeException("topCount");
            }
            
            string tableName = CheckAndGetTableName(entityType);

            if (string.IsNullOrEmpty(condition))
            {
                condition = " null=null ";
            }

            var sql = new StringBuilder();

            sql.Append("SELECT ");
            sql.Append(" TOP " + topCount.ToString(CultureInfo.CurrentCulture) + " ");
            sql.Append(ConvertAndFormatColumnNames(entityType, propertyNames));
            sql.Append(" FROM ");
            sql.Append(tableName + "(NOLOCK)");
            sql.Append(" WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            return sql.ToString();
        }
    }
}

