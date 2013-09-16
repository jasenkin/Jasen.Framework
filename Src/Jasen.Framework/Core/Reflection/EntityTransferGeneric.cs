using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Jasen.Framework.Reflection;

namespace Jasen.Framework.Reflection
{
    public static class EntityTransfer<T> where T : new()
    {
        public static void AdaptToEntity(DataTable rawTable)
        {
            EntityTransfer.AdaptToEntity(rawTable, typeof(T));
        }

        public static void AdaptToDatabase(DataTable adaptedTable)
        {
            EntityTransfer.AdaptToDatabase(adaptedTable, typeof(T));
        }

        public static IList<T> ToEntities(DataTable table, bool adaptToEntity = false)
        {
            if (table == null)
            {
                return new List<T>();
            }

            if (adaptToEntity)
            {
                EntityTransfer.AdaptToEntity(table, typeof(T));
            }

            return ToEntities(table.Rows);
        }

        public static IList<T> ToEntities(DataRowCollection adaptedRows)
        {
            if (adaptedRows == null || adaptedRows.Count <= 0)
            {
                return new List<T>();
            }

            IList<T> entities = EntityTransfer.CreateInstances<T>(adaptedRows.Count);
            CopyToEntities(entities, adaptedRows);

            return entities;
        }

        public static T ToEntity(DataRow adaptedRow)
        {
            T entity = default(T);

            if (adaptedRow == null)
            {
                return entity;
            }

            CopyToEntity(entity, adaptedRow);

            return entity;
        }

        public static void CopyToEntities(IList<T> entities, DataTable adaptedTable)
        {
            EntityTransfer.CopyToEntities(entities, adaptedTable);
        }

        public static void CopyToEntities(IList<T> entities, DataRowCollection adaptedRows)
        {
            EntityTransfer.CopyToEntities(entities, adaptedRows);
        }

        public static void CopyToEntity(T entity, DataRow adaptedRow)
        {
            EntityTransfer.CopyToEntity(entity, adaptedRow);
        }

        public static bool CanCopyToEntity(T entity, DataRow adaptedRow)
        {
            return EntityTransfer.CanCopyToEntity(entity, adaptedRow);
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

            return EntityTransfer.ToTable(entities, isAdapted);
        }

        public static DataTable CreateTable()
        {
            return CreateTable(true);
        }

        public static DataTable CreateTable(bool isAdapted)
        {
            return EntityTransfer.CreateTable(typeof(T), isAdapted);
        }

        public static string ToColumnName(string propertyName)
        {
            return EntityTransfer.ToColumnName(propertyName, typeof(T));
        }

        public static string ToPropertyName(string columnName)
        {
            return EntityTransfer.ToPropertyName(columnName, typeof(T));
        }

        public static List<T> CreateEntities(int count)
        {
            if (count <= 0)
            {
                return new List<T>();
            }

            return EntityTransfer.CreateInstances<T>(count);
        }

    }

}
