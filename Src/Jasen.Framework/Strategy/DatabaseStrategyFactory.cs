using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Strategy
{
    public class DatabaseStrategyFactory
    {
        public static DatabaseStrategy CreateStrategy(DatabaseConfig databaseConfig)
        {
            if (databaseConfig == null)
            {
                throw new ArgumentNullException("databaseConfig");
            }

            switch (databaseConfig.DatabaseType)
            {
                case DatabaseType.Oracle:
                    return new OracleStrategy(databaseConfig);
                case DatabaseType.Sqlite:
                    return new SqliteStrategy(databaseConfig);
                case DatabaseType.SqlServer:
                    return new SqlServerStrategy(databaseConfig);
                case DatabaseType.Oledb:
                    return new OledbStrategy(databaseConfig);
                default:
                    return new SqlServerStrategy(databaseConfig);
            }

        }
    }
}
