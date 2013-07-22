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
    public static class EntityMapper<T> where T : new()
    {
        public static void AdaptToEntity(DataTable rawTable)
        {
            EntityMapper.AdaptToEntity(rawTable, typeof(T));
        }

        public static void AdaptToDatabase(DataTable adaptedTable)
        {
            EntityMapper.AdaptToDatabase(adaptedTable, typeof(T));
        }
 
        public static IList<T> ToEntities(DataTable table,bool adaptToEntity= false)
        {
            if (table == null)
            {
                return new List<T>();
            }

            if(adaptToEntity)
            {
                EntityMapper.AdaptToEntity(table, typeof(T));
            }

            return ToEntities(table.Rows);
        }

        public static IList<T> ToEntities(DataRowCollection adaptedRows)
        {
            if (adaptedRows == null || adaptedRows.Count <= 0)
            {
                return new List<T>();
            }

            IList<T> entities = ObjectBuilder.CreateInstances<T>(adaptedRows.Count);
            CopyToEntities(entities, adaptedRows);

            return entities;
        }

        public static T ToEntity(DataRow adaptedRow)
        {
            T entity = ObjectBuilder.CreateInstance<T>();
            if (adaptedRow == null)
            {
                return entity;
            }

            CopyToEntity(entity, adaptedRow);

            return entity;
        }

        public static void CopyToEntities(IList<T> entities, DataTable adaptedTable)
        {
            EntityMapper.CopyToEntities(entities, adaptedTable);
        }

        public static void CopyToEntities(IList<T> entities, DataRowCollection adaptedRows)
        {
            EntityMapper.CopyToEntities(entities, adaptedRows);
        }

        public static void CopyToEntity(T entity, DataRow adaptedRow)
        {
            EntityMapper.CopyToEntity(entity, adaptedRow);
        }

        public static bool CanCopyToEntity(T entity, DataRow adaptedRow)
        {
            return EntityMapper.CanCopyToEntity(entity, adaptedRow);
        }

        public static DataTable ToTable(IList<T> entities)
        {
            return ToTable(entities, true);
        }

        public static DataTable ToTable(IList<T> entities, bool isAdapted)
        {
            if (entities == null || entities.Count <= 0 ||
                typeof(T).GetProperties().Length <= 0)
            {
                DataTable newTable = new DataTable(typeof(T).Name);
                newTable.Locale = CultureInfo.InvariantCulture;
                return newTable;
            }

            return EntityMapper.ToTable(entities, isAdapted);
        }

        public static DataTable CreateTable()
        {
            return CreateTable(true);
        }

        public static DataTable CreateTable(bool isAdapted)
        {
            return EntityMapper.CreateTable(typeof(T), isAdapted);
        }

        public static string ToColumnName(string propertyName)
        {
            return EntityMapper.ToColumnName(propertyName, typeof(T));
        }

        public static string ToPropertyName(string columnName)
        {
            return EntityMapper.ToPropertyName(columnName, typeof(T));
        }

        public static List<T> CreateEntities(int count)
        {
            if (count <= 0)
            {
                return new List<T>();
            }
            return ObjectBuilder.CreateInstances<T>(count);
        }

    }

    /// <summary>
    /// Mapping tool between entity and database.
    /// </summary>
    public static class EntityMapper
    {
        /// <summary>
        /// To keep the column names the same as the property names.
        /// </summary>
        /// <param name="rawTable">the table with the same schema of database.</param>
        /// <param name="entityType"></param>
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

        /// <summary>
        /// To keep the column names the same as the property names.
        /// </summary>
        /// <param name="rawTable"></param>
        /// <param name="columnAttributes"></param>
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

        //private static string ToUpperColumnName(string columnName)
        //{
        //    return columnName.ToUpper(CultureInfo.InvariantCulture);
        //}


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

        /// <summary>
        ///  To keep the column names the same as the schema of database.
        /// </summary>
        /// <param name="adaptedTable"></param>
        /// <param name="entityType"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adaptedTable"></param>
        /// <param name="columnAttributes"></param>
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


        /// <summary>
        ///  Creates a array of entities each of whose property's value is retrieved from rows in a table. 
        ///  The property must contain a ColumnAttribute.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IList ToEntities(DataTable table, Type entityType)
        {
            if (table == null)
            {
                return null;
            }
            return ToEntities(table.Rows, entityType);
        }

        /// <summary>
        /// Creates a array of entities each of whose property's value is retrieved from rows in a table. 
        /// The property must contain a ColumnAttribute.
        /// </summary>
        /// <param name="adaptedRows"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
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

            
            entities.AddRange(ObjectBuilder.CreateInstances(entityType, adaptedRows.Count));
            CopyToEntities(entities, adaptedRows);

            return entities;
        }


        /// <summary>
        ///  Creates a entity each of whose property's value is retrieved from a row. The property must contain a ColumnAttribute.
        /// </summary>
        /// <param name="adaptedRow"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static object ToEntity(DataRow adaptedRow, Type entityType)
        {
            if (entityType == null || adaptedRow == null)
            {
                return null;
            }

            object entity = ObjectBuilder.CreateInstance(entityType);
            CopyToEntity(entity, adaptedRow);

            return entity;
        }

        /// <summary>
        ///  Indicates if it is possible to copy values to a entity from a row.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adaptedRow"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Copies values to a entity from a row.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="adaptedRow"></param>
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


        /// <summary>
        ///  Copies values to a array of entities from rows in a table. The property must contain a ColumnAttribute.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="adaptedTable"></param>
        public static void CopyToEntities(IEnumerable entities, DataTable adaptedTable)
        {
            if (adaptedTable == null)
            {
                return;
            }

            CopyToEntities(entities, adaptedTable.Rows);
        }

        /// <summary>
        ///  Copies values to a array of entities from rows in a table. The property must contain a ColumnAttribute.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="adaptedRows"></param>
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



        /// <summary>
        /// Creates a table each of whose rows is reflected from the entity properties.The state of all rows is Added state.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static DataTable ToTable(IEnumerable entities)
        {
            return ToTable(entities, true);
        }

        /// <summary>
        /// Creates a table each of whose rows is reflected from the entity properties or database columns.The state of all rows is Added state.
        /// </summary>
        /// <param name="isAdapted"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Creates a DataTable with the same schema of the entity.
        /// </summary>
        /// <returns></returns>
        public static DataTable CreateTable(Type entityType)
        {
            return CreateTable(entityType, true);
        }

        /// <summary>
        /// Creates a DataTable with the same schema of the entity.
        /// </summary>
        /// <param name="isAdapted"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
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
                //Creates columns for the table,the column names are the same as the property names of the entity.
                CreateColumns(columnProperties, ref newTable);
            }
            else
            {
                // Creates columns for the table,the column names are the same as database columns.
                CreateColumns(columnAttributes, ref newTable);
            }
        }

        /// <summary>
        /// Creates columns for the table,the column names are the same as the property names of the entity.
        /// </summary>
        /// <param name="columnProperties"></param>
        /// <param name="table"></param>
        private static void CreateColumns(ColumnPropertyCollection columnProperties, ref DataTable table)
        {
            foreach (PropertyInfo propertyInfo in columnProperties)
            {
                DataColumn column = CreateColumn(propertyInfo.Name, propertyInfo.PropertyType);
                table.Columns.Add(column);
            }
        }

        /// <summary>
        /// Creates columns for the table,the column names are the same as database columns.
        /// </summary>
        /// <param name="columnAttributes"></param>
        /// <param name="table"></param>
        private static void CreateColumns(ColumnAttributeCollection columnAttributes, ref DataTable table)
        {
            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                DataColumn column = CreateColumn(columnAttribute.ColumnName, columnAttribute.PropertyType);
                table.Columns.Add(column);
            }
        }

        /// <summary>
        /// Creates a column with specified name and data type.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Copy values from properties to DataRow,the column names are the same as the property names of the entity.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnProperties"></param>
        /// <param name="entity"></param>
        private static void CopyValuesToDataRow(DataRow row, ColumnPropertyCollection columnProperties, object entity)
        {
            foreach (PropertyInfo propertyInfo in columnProperties)
            {
                CopyValueToRow(row, propertyInfo.Name, entity, propertyInfo);
            }
        }

        /// <summary>
        /// Copy values from properties to DataRow,the column names are the same as database columns.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="columnProperties"></param>
        /// <param name="entity"></param>
        private static void CopyValuesToDataRow(DataRow row, ColumnAttributeCollection columnAttributes,
            ColumnPropertyCollection columnProperties, object entity)
        {
            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                CopyValueToRow(row, columnAttribute.ColumnName, entity, columnProperties[columnAttribute.PropertyName]);
            }
        }

        /// <summary>
        /// Copy value from property to data cell specified by columnName.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="entity"></param>
        /// <param name="propertyInfo"></param>
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


        /// <summary>
        /// Maps property name to column name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Maps column name to property name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
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


    }

    public static class ObjectBuilder
    {
        /// <summary>
        ///  Creates a list of instances.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> CreateInstances<T>(int count)
        {
            List<T> items = new List<T>();
            for (int i = 0; i < count; i++)
            {
                items.Add(CreateInstance<T>());
            }
            return items;
        }

        /// <summary>
        ///  Creates a instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            T item;
            item = System.Activator.CreateInstance<T>();
            return item;
        }

        /// <summary>
        ///  Creates a list of instances.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static object[] CreateInstances(Type type, int count)
        {
            object[] items = new object[count];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = CreateInstance(type);
            }
            return items;
        }

        /// <summary>
        /// Creates a instance.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            return System.Activator.CreateInstance(type);
        }

    }
}
