using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Text;
using System.Reflection;

namespace Jasen.Framework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ColumnAttribute()
        {
        }
    
        public ColumnAttribute(string columnName, bool isIdentity = false, bool isPrimaryKey = false)
        {
            this.ColumnName = columnName; 
            this.IsIdentity = isIdentity;
            this.IsPrimaryKey = isPrimaryKey;
        }

        public ColumnAttribute(string columnName, bool isForeignKey, Type referenceType)
        {
            this.ColumnName = columnName;
            this.IsForeignKey = isForeignKey;
            this.ReferenceType = referenceType;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type ReferenceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(SqlDbType.NVarChar)]
        public SqlDbType SqlDbType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(OracleType.NVarChar)]
        public OracleType OracleType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(OleDbType.VarWChar)]
        public OleDbType OleDbType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool IsNullable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type PropertyType { get; set; }

        [DefaultValue(DbType.String)]
        public DbType SqliteDbType { get; set; }
    }
}
