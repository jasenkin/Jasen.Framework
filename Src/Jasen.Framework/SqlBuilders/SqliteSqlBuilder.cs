using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources; 

namespace Jasen.Framework
{
    public class SqliteSqlBuilder : SqlBuilder
    {
        public override IDataParameter CreateColumnParameter(ColumnAttribute columnAttribute,
            string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, columnAttribute.SqliteDbType);
        }

        public override IDataParameter CreateProcedureParameter(ParameterAttribute parameterAttribute, string parameterName, object parameterValue)
        {
            return CreateParameter(parameterName, parameterValue, parameterAttribute.SqliteDbType);
        }

        private static IDataParameter CreateParameter(string parameterName, object parameterValue,
            DbType dbType)
        {
            parameterValue = parameterValue ?? DBNull.Value;

            if (parameterValue is DBNull)
            {
                return new SQLiteParameter(parameterName, dbType);
            }

            return new SQLiteParameter(parameterName, parameterValue);
        }

        public override string FormatParameterList(IList<string> columnNames)
        {
            return base.FormatParameterList(columnNames, DatabaseType.Sqlite);
        }

        public override string Exists(Type entityType, string condition)
        {
            return Count(entityType, condition);
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
            return string.Concat(" SELECT last_insert_rowid() ");  // 返回INT64
        }

        protected override string FormatColumnAndParameterPairs(IList<string> columnNames)
        {
            return FormatColumnAndParameterPairs(columnNames, DatabaseType.Sqlite);
        }

        public override string SelectPaging(Type entityType, string condition,
            int pageSize, int pageIndex, Order order, IList<string> propertyNames = null)
        {
            if (pageSize <= 0)
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

            var sql = new StringBuilder();

            sql.Append("SELECT ");
            sql.Append(ConvertAndFormatColumnNames(entityType, propertyNames));
            sql.Append(" FROM ");
            sql.Append(tableName);
            sql.Append(" WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            int offset = pageSize * pageIndex;
           
            sql.Append(" LIMIT " + pageSize.ToString(CultureInfo.CurrentCulture) + " ");
            sql.Append(" OFFSET " + offset.ToString(CultureInfo.CurrentCulture) + " ");

            return sql.ToString();
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
            sql.Append(ConvertAndFormatColumnNames(entityType, propertyNames));
            sql.Append(" FROM ");
            sql.Append(tableName);
            sql.Append(" WHERE " + condition);

            if (order != null && !string.IsNullOrEmpty(order.OrderString))
            {
                sql.Append(" ORDER BY " + order.OrderString);
            }

            sql.Append(" LIMIT " + topCount.ToString(CultureInfo.CurrentCulture) + " ");

            return sql.ToString();
        }
    }
}