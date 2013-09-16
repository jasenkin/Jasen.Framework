using System; 
using System.Reflection; 
using Jasen.Framework.Core;

namespace Jasen.Framework.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class KeyUtility
    {
        public static IdentityPrimaryKeyInfo GetPrimaryKeyInfo(Type type)
        {
            if (type == null)
            {
                return null;
            }

            PropertyInfo[] propertyInfos = type.GetProperties();
            ColumnAttribute columnAttribute;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);

                if (columnAttribute != null && columnAttribute.IsPrimaryKey)
                {
                    var primaryKeyInfo = new IdentityPrimaryKeyInfo();
                    primaryKeyInfo.Name = (columnAttribute.ColumnName ?? propertyInfo.Name).Trim();
                    primaryKeyInfo.ColumnAttribute = columnAttribute;
                    primaryKeyInfo.PropertyInfo = propertyInfo;

                    return primaryKeyInfo;
                }
            }

            return null;
        }

        public static PropertyInfo GetPrimaryKeyProperty(Type type)
        {
            if (type == null)
            {
                return null;
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);
               
                if (columnAttribute != null && columnAttribute.IsPrimaryKey)
                {
                    return propertyInfo;
                }
            }

            return null;
        }

        public static string GetPrimaryKeyName(Type type)
        {
            PropertyInfo propertyInfo = GetPrimaryKeyProperty(type);

            if (propertyInfo == null)
            {
                return string.Empty;
            }

            return AttributeUtility.GetColumnName(propertyInfo);
        }

        public static void GetPrimaryKey(object entity, out string key, out object value)
        {
            key = string.Empty;
            value = null;

            if (entity == null)
            {
                return;
            }

            PropertyInfo propertyInfo = GetPrimaryKeyProperty(entity.GetType());

            if (propertyInfo == null)
            {
                return;
            }

            key = AttributeUtility.GetColumnName(propertyInfo);
            value = propertyInfo.GetValue(entity, null);
        }

        public static PropertyInfo GetForeignKeyProperty(Type type, Type referenceType)
        {
            if (type == null || referenceType == null)
            {
                return null;
            }

            ColumnAttribute columnAttribute = null;

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);
                if (columnAttribute != null && columnAttribute.IsForeignKey &&
                    columnAttribute.ReferenceType == referenceType)
                {
                    return propertyInfo;
                }
            }

            return null;
        }

        public static string GetForeignKey(Type type, Type referenceType)
        {
            if (type == null || referenceType == null)
            {
                return string.Empty;
            }

            ColumnAttribute columnAttribute = null;

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);
                if (columnAttribute != null && columnAttribute.IsForeignKey && 
                    columnAttribute.ReferenceType == referenceType)
                {
                    return columnAttribute.ColumnName;
                }
            }

            return string.Empty;
        }

    }
}
