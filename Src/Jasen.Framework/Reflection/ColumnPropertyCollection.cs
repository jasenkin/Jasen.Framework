using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection; 

namespace Jasen.Framework.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnPropertyCollection : IEnumerable<PropertyInfo>
    {
        private Dictionary<string, PropertyInfo> _columnProperties = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public ColumnPropertyCollection(Type type)
        {
            this.GetConfiguration(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo this[string propertyName]
        {
            get
            {
                if (_columnProperties.ContainsKey(propertyName))
                {
                    return _columnProperties[propertyName];
                }
                else
                {
                    return null;
                }
            }
        }


        #region IEnumerable<ColumnAttribute> Members

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            foreach (PropertyInfo propertyInfo in _columnProperties.Values)
            {
                yield return propertyInfo;
            }
        }

        #endregion




        #region Custom Methods

        //public bool ContainsKey(string propertyName)
        //{
        //    return _columnProperties.ContainsKey(propertyName);
        //}

        public void GetConfiguration(Type type)
        {
            if (type == null)
            {
                return;
            }

            ColumnAttribute columnAttribute = null;
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                columnAttribute = AttributeUtility.GetColumnAttribute(propertyInfo);
                if (columnAttribute != null)
                {
                    if (!_columnProperties.ContainsKey(propertyInfo.Name))
                    {
                        _columnProperties.Add(propertyInfo.Name, propertyInfo);
                    }
                }
            }

        }
        
        #endregion


        #region IEnumerable ≥…‘±

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _columnProperties.GetEnumerator();
        }

        #endregion
    }
}
