using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using Jasen.Framework;
using Jasen.Framework.Attributes;

namespace Jasen.Framework.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class AttributeUtility
    { 
        public static T GetCustomAttribute<T>(PropertyInfo propertyInfo)
        {
            object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            if (customAttributes.Length > 0)
            {
                return (T)customAttributes[0];
            }
            return default(T);
        }

        #region Column

        public static ColumnAttribute GetColumnAttribute(PropertyInfo propertyInfo)
        {
            object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
            ColumnAttribute parameterAttribute;

            foreach (object attr in customAttributes)
            {
                parameterAttribute = attr as ColumnAttribute;

                if (parameterAttribute != null)
                {
                    return parameterAttribute;
                }
            }

            return null;
        }

        public static ColumnAttribute GetColumnAttribute(Type type, string propertyName)
        {
            if (type == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (string.Equals(propertyInfo.Name, propertyName))
                {
                    return GetColumnAttribute(propertyInfo);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string GetColumnName(PropertyInfo propertyInfo)
        {
            ColumnAttribute columnAttribute = GetColumnAttribute(propertyInfo);

            if (columnAttribute == null)
            {
                return "";
            }

            if (!string.IsNullOrEmpty(columnAttribute.ColumnName))
            {
                return columnAttribute.ColumnName;
            }
            else
            {
                return propertyInfo.Name;
            }
        }

        #endregion


        #region Table

        public static TableAttribute GetTableAttribute(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            TableAttribute tableAttribute = null;
            foreach (object attr in customAttributes)
            {
                tableAttribute = attr as TableAttribute;
                if (tableAttribute != null)
                {
                    return tableAttribute;
                }
            }
            return null;
        }

        public static DatabaseType GetDatabaseType(string providerName)
        {  
            if(string.IsNullOrWhiteSpace(providerName))
            {
                return DatabaseType.None;
            }
 
            ProviderAttribute attribute;

            foreach (FieldInfo fieldInfo in typeof(DatabaseType).GetFields())
            {
                object[] customAttributes = fieldInfo.GetCustomAttributes(typeof (ProviderAttribute), true);

                foreach (object attr in customAttributes)
                {
                    attribute = attr as ProviderAttribute;

                    if (attribute==null || attribute.Name == null)
                    {
                        continue;
                    }

                    foreach (string name in attribute.Name)
                    {
                        if (string.Equals(providerName.Trim(), name.Trim()))
                        {
                            return TryParse(fieldInfo.Name);
                        }
                    }
                }
            }

            return DatabaseType.None;
        }

        private static DatabaseType TryParse(string typeName)
        {
            DatabaseType databaseType;
            Enum.TryParse(typeName, out databaseType);

            if (Enum.IsDefined(typeof(DatabaseType), databaseType))
            {
                return databaseType;
            }

            return DatabaseType.None;
        } 

        public static string GetTableName(Type type)
        {
            TableAttribute attr = GetTableAttribute(type);

            if (attr != null)
            {
                return attr.TableName;
            }

            return type.Name;
        }

        #endregion

        public static ParameterAttribute GetParameterAttribute(PropertyInfo propertyInfo)
        {
            object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true);
            ParameterAttribute attribute = null;
            foreach (object attr in customAttributes)
            {
                attribute = attr as ParameterAttribute;
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static ProcedureAttribute GetProcedureAttribute(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(ProcedureAttribute), true);
            ProcedureAttribute attribute = null;

            foreach (object attr in customAttributes)
            {
                attribute = attr as ProcedureAttribute;

                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
        }

        public static IDictionary<ParameterAttribute,PropertyInfo> GetParameterAndProperties(Type type)
        {
            var dictionary = new Dictionary<ParameterAttribute, PropertyInfo>();
            ParameterAttribute attribute;

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                attribute = GetParameterAttribute(propertyInfo);

                if (attribute != null)
                {
                    dictionary.Add(attribute, propertyInfo);
                }
            }

            return dictionary;
        }

        internal static PropertyAssociation GetPropertyAssociation(Type type, string propertyName)
        {
            if (type == null||string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
 
            var propertyInfo = type.GetProperty(propertyName);

            if(propertyInfo==null)
            {
                return null;
            }

            object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(AssociationAttribute), true);

            foreach (object attr in customAttributes)
            {
                var attribute = attr as AssociationAttribute;
                if (attribute != null)
                {
                    return new PropertyAssociation(propertyInfo, attribute);
                }
            }

            return null;
        }

        internal static IList<PropertyAssociation> GetAllPropertyAssociations(Type type, params string[] propertyNames)
        {
            if (type == null||propertyNames==null||propertyNames.Length==0)
            {
                return new List<PropertyAssociation>();
            }

            var associations = new List<PropertyAssociation>();
            PropertyAssociation currAssociation;

            foreach(var propertyName in propertyNames)
            {
                if(string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }

                currAssociation =  GetPropertyAssociation(type, propertyName.Trim());

                if(currAssociation!=null)
                {
                    associations.Add(currAssociation);
                }
            }

            return associations;
        }

        internal static IList<PropertyInfo> GetAssociationProperties(Type type)
        {
            if (type == null)
            {
                return new List<PropertyInfo>();
            }

            var result = new List<PropertyInfo>();
            AssociationAttribute attribute = null;

            foreach (var propertyInfo in type.GetProperties())
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(AssociationAttribute), true);

                foreach (object attr in customAttributes)
                {
                    attribute = attr as AssociationAttribute;
                    if (attribute != null)
                    {
                        result.Add(propertyInfo);
                    }
                } 
            } 
            return result;
        }
    }
}
