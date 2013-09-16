using System;
using System.Reflection;
using Jasen.Framework.Core;

namespace Jasen.Framework.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdentityUtility
    {
        public static PropertyInfo GetIdentityProperty(Type type)
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
             
                if (columnAttribute != null && columnAttribute.IsIdentity)
                { 
                    return propertyInfo;
                }
            }

            return null;
        }

        public static IdentityPrimaryKeyInfo GetIdentityInfo(Type type)
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

                if (columnAttribute != null && columnAttribute.IsIdentity)
                {
                    IdentityPrimaryKeyInfo identityInfo = new IdentityPrimaryKeyInfo();
                    identityInfo.Name = (columnAttribute.ColumnName ?? propertyInfo.Name).Trim();
                    identityInfo.ColumnAttribute = columnAttribute;
                    identityInfo.PropertyInfo = propertyInfo;

                    return identityInfo;
                }
            }

            return null;
        }
         
        public static string GetIdentityName(Type type)
        {
            PropertyInfo propertyInfo = GetIdentityProperty(type);

            if (propertyInfo == null)
            {
                return string.Empty;
            }

            return AttributeUtility.GetColumnName(propertyInfo);
        }

        public static void GetIdentity(object entity, out string identityName, out object identityValue)
        {
            identityName = string.Empty;
            identityValue = null;
            if (entity == null)
            {
                return;
            }

            PropertyInfo propertyInfo = GetIdentityProperty(entity.GetType());

            if (propertyInfo == null)
            {
                return;
            }

            identityName = AttributeUtility.GetColumnName(propertyInfo);

            identityValue = propertyInfo.GetValue(entity, null);
        }
    }
}



