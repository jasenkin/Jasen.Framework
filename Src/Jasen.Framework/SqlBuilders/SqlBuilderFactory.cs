using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    public class SqlBuilderFactory
    {
        public static ISqlBuilder CreateInstance(DatabaseType databaseType)
        {
            ISqlBuilder builder = null;

            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    builder = new SqlServerSqlBuilder();
                    break;
                case DatabaseType.Oracle:
                    builder = new OracleSqlBuilder();
                    break;
                case DatabaseType.Oledb:
                    builder = new OledbSqlBuilder();
                    break;
                case DatabaseType.Sqlite:
                    builder = new SqliteSqlBuilder();
                    break;
                default:
                    builder = new SqlServerSqlBuilder();
                    break;
            }

            return builder;
        } 
    }
}
