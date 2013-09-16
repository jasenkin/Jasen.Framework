using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using Jasen.Framework;

namespace Jasen.Framework.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnAttributeCollection : IEnumerable<ColumnAttribute>
    {
        private readonly Dictionary<string, ColumnAttribute> _columnAttributes = new Dictionary<string, ColumnAttribute>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public ColumnAttributeCollection(Type type)
        {
            this.GetConfiguration(type);
        }

        public ColumnAttribute this[string columnName]
        {
            get
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return null;
                }

                if (!_columnAttributes.ContainsKey(columnName))
                {
                    return null;
                }

                return _columnAttributes[columnName];
            }
        }

        public IEnumerator<ColumnAttribute> GetEnumerator()
        {
            foreach (ColumnAttribute attr in _columnAttributes.Values)
            {
                yield return attr;
            }
        }

        #region Custom Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool ContainsKey(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return false;
            }

            return _columnAttributes.ContainsKey(columnName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void GetConfiguration(Type type)
        {

            if (type == null || 
                type.GetProperties().Length <= 0)
            {
                return;
            }

            ColumnAttribute attr = null;
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                attr = AttributeUtility.GetColumnAttribute(propertyInfo);
                if (attr != null)
                {
                    attr.PropertyName = propertyInfo.Name;
                    attr.PropertyType = propertyInfo.PropertyType;
                    if (!_columnAttributes.ContainsKey(attr.ColumnName))
                    {
                        _columnAttributes.Add(attr.ColumnName, attr);
                    }
                }
            }

        }
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get 
            { 
                return _columnAttributes.Count; 
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _columnAttributes.GetEnumerator();
        }

    }
}
