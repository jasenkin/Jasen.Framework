using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Jasen.Framework.Resources;
using System.Data;
using Jasen.Framework.Reflection;
using System.Reflection;
using Jasen.Framework.Core;
using MySql.Data.MySqlClient;

namespace Jasen.Framework
{
    /// <summary>
    /// the operation of MySql is the same as sqlite
    /// </summary>
    public class MySqlGenerator : SqlGenerator
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
                return new MySqlParameter(parameterName, dbType);
            }

            return new MySqlParameter(parameterName, parameterValue);
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

        /// <summary>
        /// 与SQLITE类似，可以采用LAST_INSERT_ID函数
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public override string SelectCurrentIdentity(Type entityType)
        {
            return string.Concat("SELECT LAST_INSERT_ID();");
        }

        protected override string FormatColumnAndParameterPairs(IList<string> columnNames)
        {
            return FormatColumnAndParameterPairs(columnNames, SQL_PARAMETER_PREFIX_SIGN);
        }

        /// <summary>
        /// LIKE SQLITE ,Use LIMIT command to get the page content
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="condition"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="order"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
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
       
        /// <summary>
        /// LIKE SQLITE ,Use LIMIT command to get the top content
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="condition"></param>
        /// <param name="topCount"></param>
        /// <param name="order"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
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
