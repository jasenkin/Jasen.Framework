using System;
using System.Data;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using Jasen.Framework.Resources;
using Jasen.Framework;
using Jasen.Framework.Core;
using Jasen.Framework.Reflection;

namespace Jasen.Framework
{
    public class OledbSqlBuilder : SqlBuilder
    {
        public override IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute,
            string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, columnAttribute.OleDbType);
        }

        public override IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute, string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, parameterAttribute.OleDbType);
        }

        private static IDataParameter CreateParameter(string parameterName, object parameterValue,
            OleDbType dbType)
        {
            parameterValue = parameterValue ?? DBNull.Value;

            if (parameterValue is DBNull)
            {
                return new OleDbParameter(parameterName, dbType);
            }

            return new OleDbParameter(parameterName, parameterValue);
        } 

        public override string FormatParameterList(IList<string> columnNames)
        {
            return base.FormatParameterList(columnNames, DatabaseType.Oledb);
        }

        public override string Exists(Type entityType, string condition)
        {
            return Count(entityType ,condition);
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
            string tableName = CheckAndGetTableName(entityType);

            string identityName = IdentityUtility.GetIdentityName(entityType);

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(MsgResource.IdentityOfEntityMissing);
            }

            return string.Concat("SELECT MAX(", identityName, ") FROM ", tableName);
        } 

        protected override string FormatColumnAndParameterPairs(IList<string> columnNames)
        {
            return FormatColumnAndParameterPairs(columnNames, DatabaseType.Oledb);
        }
 
        public override string SelectPaging(Type entityType, string condition,  
            int pageSize, int pageIndex, Order order, IList<string> propertyNames = null)
        {
            if(pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
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

            string columnNameList = ConvertAndFormatColumnNames(entityType, propertyNames);

            pageIndex = pageIndex < 0 ? 0 : pageIndex;

            if (pageIndex == 0)
            {
                return "SELECT TOP " + pageSize + " " + columnNameList + " FROM " + tableName +
                      " WHERE " + condition + " ORDER BY " + order.OrderString;
            }

            return "SELECT TOP " + pageSize + " " + columnNameList + " FROM " + tableName +
                      " WHERE " + condition + " AND " + order.OrderColumn +
                      " NOT IN (SELECT TOP " + pageSize * pageIndex + " " + order.OrderColumn + " FROM " + tableName +
                      " WHERE " + condition + " ORDER BY " + order.OrderString + ") ORDER BY " + order.OrderString;
        }

        public override string SelectTop(Type entityType, string condition, int topCount,
            Order order = null, IList<string> propertyNames = null)
        {
            if(topCount<=0)
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
            sql.Append(tableName);
            sql.Append(" WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            return sql.ToString();
        }
 
    }
}
