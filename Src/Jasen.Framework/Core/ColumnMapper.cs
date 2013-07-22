using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Jasen.Framework;

namespace Jasen.Framework.Reflection
{
    internal static class ColumnMapper
    {

        #region ReflectColumns

        /// <summary>
        /// Format : column1,column2,column3...
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyNames"></param>
        /// <returns>column1,column2,column3...</returns>
        public static string SelectColumnNameList(Type entityType, IList<string> propertyNames = null)
        {
            List<string> columns;

            if(propertyNames==null||propertyNames.Count==0)
            {
                columns = SelectAllColumnNameList(entityType);
            }
            else
            {
                columns = ConvertPropertiesToColumnNames(entityType, propertyNames);
            } 

            return FormatColumnNames(columns);
        }

        /// <summary>
        /// Format : column1,column2,column3...
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyNames"></param>
        /// <returns>column1,column2,column3...</returns>
        public static string InsertColumnNameList(Type entityType, IList<string> propertyNames = null)
        {
            List<string> columns = ConvertPropertiesToColumnNames(entityType, propertyNames, true);
            return FormatColumnNames(columns);
        }

        /// <summary>
        /// Sql Server Format : @column1,@column2,@column3...
        /// Oracle Format : :column1,:column2,:column3...
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="entityType"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static string InsertParameterList(DatabaseType databaseType, Type entityType, IList<string> propertyNames)
        {
            List<string> columns = ConvertPropertiesToColumnNames(entityType, propertyNames, true);
            return FormatParameterList(columns, databaseType);
        }

        /// <summary>
        /// Sql Server Format : column1=@column1,column2=@column3,column3=@column3...
        /// Oracle Format : column1=:column1,column2=:column3,column3=:column3...
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="entityType"></param>
        /// <param name="updatedByPK"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static string UpdateColumnParameterPairs(DatabaseType databaseType, Type entityType,
            bool updatedByPK, List<string> propertyNames)
        {
            List<string> columns = PropertiesToUpdateColumnNames(entityType, updatedByPK, propertyNames);
            return BuildColumnAndParameterPairs(columns, databaseType);
        }


        #endregion

        #region Private Utility

        /// <summary>
        /// Reflect Column Name from entity's properite which contains a ColumnAttribute.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public  static List<string> SelectAllColumnNameList(Type entityType)
        {
            List<string> columnNames = new List<string>();
            if (entityType == null || entityType.GetProperties().Length <= 0)
            {
                return columnNames;
            }

            string columnName = "";
            ColumnAttribute columnAttribute = null;
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
            IEnumerable<string> propertyNames,bool isInsertSql = false)
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
                var propertyInfo =  entityType.GetProperty(propertyName);

                if(propertyInfo==null)
                {
                    continue;
                }

                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute != null)
                {
                    // 如果是Insert语句，并且为标识列，忽略该列。
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

       
        /// <summary>
        /// PropertiesToColumnNames for updating.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="updatedByPK"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        private static List<string> PropertiesToUpdateColumnNames(Type entityType, bool updatedByPK, List<string> propertyNames)
        {
            List<string> columnNames = new List<string>();
            if (entityType == null || entityType.GetProperties().Length <= 0)
            {
                return columnNames;
            }

            string columnName = "";
            ColumnAttribute columnAttribute = null;
            foreach (string propertyName in propertyNames)
            {
                foreach (PropertyInfo propertyInfo in entityType.GetProperties())
                {
                    if (propertyInfo.Name != propertyName)
                    {
                        continue;
                    }
                    columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);
                    if (columnAttribute != null)
                    {
                        //Identity property will be excluded when updating.
                        if (columnAttribute.IsIdentity)
                        {
                            continue;
                        }
                        if (columnAttribute.IsPrimaryKey && updatedByPK)
                        {
                            continue;
                        }
                        columnName = string.IsNullOrEmpty(columnAttribute.ColumnName) ? propertyInfo.Name : columnAttribute.ColumnName;
                        columnNames.Add(columnName);
                    }
                }
            }
            return columnNames;
        }

        public static string FormatColumnNames(IList<string> columnNames)
        {
            if(columnNames==null||columnNames.Count==0)
            {
                return string.Empty;
            }

            return string.Join(",", columnNames);
        }

        public static string FormatParameterList(IList<string> columnNames, DatabaseType databaseType)
        {
            if (columnNames == null || columnNames.Count == 0)
            {
                return string.Empty;
            }

            for(int index=0;index<columnNames.Count;index++)
            {
                if (databaseType == DatabaseType.Oracle)
                {
                    columnNames[index] = ":" + columnNames[index];
                }
                else
                {
                    columnNames[index] = "@" + columnNames[index];
                }
            }

            return FormatColumnNames(columnNames);
        }

        public static string FormatColumnAndParameterPairs(List<string> columnNames, DatabaseType databaseType)
        {
            if (columnNames == null || columnNames.Count == 0)
            {
                return string.Empty;
            }

            for (int index = 0; index < columnNames.Count; index++)
            {
                if (databaseType == DatabaseType.Oracle)
                {
                    columnNames[index] = columnNames[index] + "=:" + columnNames[index];
                }
                else
                {
                    columnNames[index] = columnNames[index] + "=@" + columnNames[index];
                }
            }

            return FormatColumnNames(columnNames);
        }

        /// <summary>
        ///  Sql Server Format : column1=@column1,column2=@column3,column3=@column3...
        ///  Oracle Format : column1=:column1,column2=:column3,column3=:column3...
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        private static string BuildColumnAndParameterPairs(List<string> columnNames, DatabaseType databaseType)
        {
            StringBuilder nameList = new StringBuilder();
            foreach (string columnName in columnNames)
            {
                nameList.Append(columnName);
                if (databaseType == DatabaseType.Oracle)
                {
                    nameList.Append("=:" + columnName);
                }
                else
                {
                    nameList.Append("=@" + columnName);
                }
                nameList.Append(",");
            }

            if (nameList.Length > 0)
            {
                nameList.Remove(nameList.Length - 1, 1);
            }
            return nameList.ToString();
        }

        #endregion

    }
}
