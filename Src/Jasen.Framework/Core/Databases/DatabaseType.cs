using System;
using Jasen.Framework.Attributes;

namespace Jasen.Framework
{
    
    [Serializable]
    public enum DatabaseType
    { 
        /// <summary>
        /// 
        /// </summary>
        [Provider("System.Data.SqlClient")]
        SqlServer,
        /// <summary>
        /// 
        /// </summary>
        [Provider("System.Data.SqlClient")]
        SqlServer2000, 
        /// <summary>
        /// 
        /// </summary>
        [Provider("System.Data.OracleClient", "Oracle.DataAccess.Client")]
        Oracle, 
        /// <summary>
        /// 
        /// </summary>
        [Provider("System.Data.OleDb")]
        Oledb,
        /// <summary>
        /// 
        /// </summary>
        [Provider("System.Data.SQLite")]
        Sqlite,
        /// <summary>
        /// 
        /// </summary>  
        MySql,
        /// <summary>
        /// 
        /// </summary>
        None

    }
}
