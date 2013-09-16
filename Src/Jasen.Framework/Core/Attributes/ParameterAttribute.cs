using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Text;

namespace Jasen.Framework
{
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParameterAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ParameterAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ParameterAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

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
        [DefaultValue(DbType.String)]
        public DbType SqliteDbType { get; set; }

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
        [DefaultValue(ParameterDirection.InputOutput)]
        public ParameterDirection Direction { get; set; }
       
    }
}
