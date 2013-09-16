using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection; 
using Jasen.Framework.Reflection;

namespace Jasen.Framework
{
     
    public static class EntityTransfer
    {
        public static void AdaptToEntity(DataTable rawTable, Type entityType)
        {
            if (!ValidateArgs(rawTable, entityType))
            {
                return;
            }

            rawTable.TableName = entityType.Name;

            ColumnAttributeCollection columnAttributes = new ColumnAttributeCollection(entityType);

            AdaptToEntity(rawTable, columnAttributes);
        }

        public static void AdaptToEntity(DataTable rawTable, IEnumerable<ColumnAttribute> columnAttributes)
        {
            if (!ValidateArgs(rawTable, columnAttributes))
            {
                return;
            }

            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                if (rawTable.Columns.Contains(columnAttribute.ColumnName))
                {
                    rawTable.Columns[columnAttribute.ColumnName].ColumnName = columnAttribute.PropertyName;
                }
            }
        }

        private static bool IsValidTable(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Columns.Count <= 0)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateArgs(DataTable dataTable, Type entityType)
        {
            if (!IsValidTable(dataTable))
            {
                return false;
            }

            if (entityType == null)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateArgs(DataTable dataTable,
            IEnumerable<ColumnAttribute> columnAttributes)
        {
            if (!IsValidTable(dataTable))
            {
                return false;
            }

            if (columnAttributes == null)
            {
                return false;
            }

            return true;
        }

        public static void AdaptToDatabase(DataTable adaptedTable, Type entityType)
        {
            if (!ValidateArgs(adaptedTable, entityType))
            {
                return;
            }

            TableAttribute tableAttribute = AttributeUtility.GetTableAttribute(entityType);
            if (tableAttribute != null)
            {
                adaptedTable.TableName = tableAttribute.TableName;
            }

            ColumnAttributeCollection columnAttributes = new ColumnAttributeCollection(entityType);

            AdaptToDatabase(adaptedTable, columnAttributes);

        }

        public static void AdaptToDatabase(DataTable adaptedTable, IEnumerable<ColumnAttribute> columnAttributes)
        {
            if (!ValidateArgs(adaptedTable, columnAttributes))
            {
                return;
            }

            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                if (adaptedTable.Columns.Contains(columnAttribute.PropertyName))
                {
                    adaptedTable.Columns[columnAttribute.PropertyName].ColumnName = columnAttribute.ColumnName;
                }
            }

        }

        public static IList ToEntities(DataTable table, Type entityType)
        {
            if (table == null)
            {
                return null;
            }
            return ToEntities(table.Rows, entityType);
        }

        public static IList ToEntities(DataRowCollection adaptedRows, Type entityType)
        {
            if (entityType == null || adaptedRows == null)
            {
                return null;
            }

            ArrayList entities = new ArrayList();
            if (adaptedRows.Count <= 0)
            {
                return entities;
            }

            
            entities.AddRange(CreateInstances(entityType, adaptedRows.Count));
            CopyToEntities(entities, adaptedRows);

            return entities;
        }

        public static object ToEntity(DataRow adaptedRow, Type entityType)
        {
            if (entityType == null || adaptedRow == null)
            {
                return null;
            }

            object entity = Activator.CreateInstance(entityType);
            CopyToEntity(entity, adaptedRow);

            return entity;
        }

        public static bool CanCopyToEntity(object entity, DataRow adaptedRow)
        {
            if (entity == null || adaptedRow == null)
            {
                return false;
            }
            PropertyInfo[] propertyInfos = entity.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (CanSetPropertyValue(propertyInfo, adaptedRow))
                {
                    return true;
                }
            }

            return false;
        }

        public static void CopyToEntity(object entity, DataRow adaptedRow)
        {
            if (entity == null || adaptedRow == null)
            {
                return;
            }
            PropertyInfo[] propertyInfos = entity.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!CanSetPropertyValue(propertyInfo, adaptedRow))
                {
                    continue;
                }

                try
                {
                    if (adaptedRow[propertyInfo.Name] is DBNull)
                    {
                        propertyInfo.SetValue(entity, null, null);
                        continue;
                    }

                    SetPropertyValue(entity, adaptedRow, propertyInfo);
                }
                finally
                {

                }
            }

        }

        private static bool CanSetPropertyValue(PropertyInfo propertyInfo, DataRow adaptedRow)
        {
            if (!propertyInfo.CanWrite)
            {
                return false;
            }

            if (!adaptedRow.Table.Columns.Contains(propertyInfo.Name))
            {
                return false;
            }

            return true;
        }

        private static void SetPropertyValue(object entity, DataRow adaptedRow, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(DateTime?) ||
                propertyInfo.PropertyType == typeof(DateTime))
            {
                DateTime date = DateTime.MaxValue;
                DateTime.TryParse(adaptedRow[propertyInfo.Name].ToString(),
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

                propertyInfo.SetValue(entity, date, null);
            }
            else
            {
                propertyInfo.SetValue(entity, adaptedRow[propertyInfo.Name], null);
            }
        }

        public static void CopyToEntities(IEnumerable entities, DataTable adaptedTable)
        {
            if (adaptedTable == null)
            {
                return;
            }

            CopyToEntities(entities, adaptedTable.Rows);
        }

        public static void CopyToEntities(IEnumerable entities, DataRowCollection adaptedRows)
        {
            if (entities == null || adaptedRows == null)
            {
                return;
            }

            int i = 0;
            foreach (var entity in entities)
            {
                if (i >= adaptedRows.Count)
                {
                    return;
                }

                CopyToEntity(entity, adaptedRows[i]);
                i++;
            }
        }

        public static DataTable ToTable(IEnumerable entities)
        {
            return ToTable(entities, true);
        }

        public static DataTable ToTable(IEnumerable entities, bool isAdapted)
        {
            if (entities == null)
            {
                return null;
            }

            Type entityType = GetTypeOfEntities(entities);
            if (entityType == null)
            {
                return new DataTable();
            }

            DataTable newTable = CreateEmptyDataTable(entityType);
            if (entityType.GetProperties().Length <= 0)
            {
                return newTable;
            }

            ColumnPropertyCollection columnProperties = new ColumnPropertyCollection(entityType);
            ColumnAttributeCollection columnAttributes = new ColumnAttributeCollection(entityType);
            CreateColumns(isAdapted, ref newTable, columnProperties, columnAttributes);

            foreach (object entity in entities)
            {
                DataRow row = newTable.NewRow();

                CopyToDataRow(isAdapted, columnProperties, columnAttributes, ref row, entity);

                newTable.Rows.Add(row);
            }

            return newTable;
        }

        private static Type GetTypeOfEntities(IEnumerable entities)
        {
            foreach (object entity in entities)
            {
                if (entity != null)
                {
                    return entity.GetType();
                }
            }

            return null;
        }

        private static DataTable CreateEmptyDataTable(Type entityType)
        {
            DataTable newTable = new DataTable();
            newTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
            newTable.TableName = entityType.Name;
            return newTable;
        }

        private static void CopyToDataRow(bool isAdapted, ColumnPropertyCollection columnProperties,
            ColumnAttributeCollection columnAttributes, ref DataRow row, object entity)
        {
            if (isAdapted)
            {
                CopyValuesToDataRow(row, columnProperties, entity);
            }
            else
            {
                CopyValuesToDataRow(row, columnAttributes, columnProperties, entity);
            }
        }

        public static DataTable CreateTable(Type entityType)
        {
            return CreateTable(entityType, true);
        }

        public static DataTable CreateTable(Type entityType, bool isAdapted)
        {
            if (entityType == null)
            {
                return null;
            }

            DataTable newTable = new DataTable();
            newTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

            SetTableName(isAdapted, entityType, newTable);

            ColumnPropertyCollection columnProperties = new ColumnPropertyCollection(entityType);
            ColumnAttributeCollection columnAttributes = new ColumnAttributeCollection(entityType);

            CreateColumns(isAdapted, ref newTable, columnProperties, columnAttributes);

            return newTable;
        }

        private static void SetTableName(bool isAdapted, Type entityType, DataTable newTable)
        {
            if (isAdapted)
            {
                newTable.TableName = entityType.Name;
            }
            else
            {
                TableAttribute tableAttribute = AttributeUtility.GetTableAttribute(entityType);
                newTable.TableName = tableAttribute == null ? "" : tableAttribute.TableName;
            }
        }

        private static void CreateColumns(bool isAdapted, ref DataTable newTable,
           ColumnPropertyCollection columnProperties, ColumnAttributeCollection columnAttributes)
        {
            if (isAdapted)
            {
                CreateColumns(columnProperties, ref newTable);
            }
            else
            {
                CreateColumns(columnAttributes, ref newTable);
            }
        }

        private static void CreateColumns(ColumnPropertyCollection columnProperties, ref DataTable table)
        {
            foreach (PropertyInfo propertyInfo in columnProperties)
            {
                DataColumn column = CreateColumn(propertyInfo.Name, propertyInfo.PropertyType);
                table.Columns.Add(column);
            }
        }

        private static void CreateColumns(ColumnAttributeCollection columnAttributes, ref DataTable table)
        {
            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                DataColumn column = CreateColumn(columnAttribute.ColumnName, columnAttribute.PropertyType);
                table.Columns.Add(column);
            }
        }

        private static DataColumn CreateColumn(string columnName, Type propertyType)
        {
            Type[] genericArguments;

            DataColumn column = new DataColumn(columnName);

            if (propertyType == typeof(Nullable))
            {
                column.DataType = Nullable.GetUnderlyingType(propertyType);
            }
            else if (propertyType.IsGenericType)
            {
                genericArguments = propertyType.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    column.DataType = genericArguments[0];
                }
            }
            else
            {
                column.DataType = propertyType;
            }

            return column;
        }

        private static void CopyValuesToDataRow(DataRow row, ColumnPropertyCollection columnProperties, object entity)
        {
            foreach (PropertyInfo propertyInfo in columnProperties)
            {
                CopyValueToRow(row, propertyInfo.Name, entity, propertyInfo);
            }
        }

        private static void CopyValuesToDataRow(DataRow row, ColumnAttributeCollection columnAttributes,
            ColumnPropertyCollection columnProperties, object entity)
        {
            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                CopyValueToRow(row, columnAttribute.ColumnName, entity, columnProperties[columnAttribute.PropertyName]);
            }
        }

        private static void CopyValueToRow(DataRow row, string columnName, object entity, PropertyInfo propertyInfo)
        {
            object propertyValue;
            propertyValue = propertyInfo.GetValue(entity, null);
            if (propertyValue == null)
            {
                row[columnName] = DBNull.Value;
            }
            else
            {
                row[columnName] = propertyValue;
            }
        }

        public static string ToColumnName(string propertyName, Type entityType)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            if (entityType == null)
            {
                return null;
            }

            ColumnAttribute columnAttribute = AttributeUtility.GetColumnAttribute(entityType, propertyName);
            if (columnAttribute == null)
            {
                return null;
            }
            return columnAttribute.ColumnName;
        }

        public static string ToPropertyName(string columnName, Type entityType)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return null;
            }

            if (entityType == null)
            {
                return null;
            }

            ColumnAttributeCollection columnAttributeCollection = new ColumnAttributeCollection(entityType);
            ColumnAttribute columnAttribute = columnAttributeCollection[columnName];
            if (columnAttribute == null)
            {
                return null;
            }
            return columnAttribute.PropertyName;
        }

         public static List<T> CreateInstances<T>(int count)
        {
            List<T> items = new List<T>();
            for (int i = 0; i < count; i++)
            {
                items.Add(Activator.CreateInstance<T>());
            }
            return items;
        }

        public static object[] CreateInstances(Type type, int count)
        {
            object[] items = new object[count];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = Activator.CreateInstance(type);
            }
            return items;
        } 
    }
}
