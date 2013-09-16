using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.Resources;

namespace Jasen.Framework
{
    public class SqlServer2000SqlGenerator : SqlServerSqlGenerator
    {
        public override string SelectPaging(Type entityType, string condition, int pageSize, int pageIndex,
           Order order, IList<string> propertyNames)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            string tableName = CheckAndGetTableName(entityType);

            if (order == null || string.IsNullOrEmpty(order.OrderColumn) || string.IsNullOrEmpty(order.OrderString))
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

            return "SELECT TOP " + pageSize + " " + columnNameList + " FROM " + tableName +
                      " WHERE " + condition + " AND " + order.OrderColumn +
                      " NOT IN (SELECT TOP " + pageSize * pageIndex + " " + order.OrderColumn + " FROM " + tableName +
                      " WHERE " + condition + " ORDER BY " + order.OrderString + ") ORDER BY " + order.OrderString;
        }

    }
}
