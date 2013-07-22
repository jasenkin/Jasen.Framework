using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public TableAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdSequence { get; set; }

    }
}
