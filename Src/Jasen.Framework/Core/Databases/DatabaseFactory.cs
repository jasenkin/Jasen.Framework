using System;
using System.Collections.Generic;
using System.Text;
using Jasen.Framework.Core.Databases;

namespace Jasen.Framework
{ 
    public static class DatabaseFactory
    { 
        public static IDatabase CreateSqlServerDatabase()
        {
            return new SqlServerDatabase();
        }
         
        public static IDatabase CreateSqlServerDatabase(string connectionString)
        {
            IDatabase database = new SqlServerDatabase();
            database.ConnectionString = connectionString;
            return database;
        }
         
        public static IDatabase CreateOracleDatabase()
        {
            return new OracleDatabase();
        }
         
        public static IDatabase CreateOracleDatabase(string connectionString)
        {
            IDatabase database = new OracleDatabase();
            database.ConnectionString = connectionString;
            return database;
        } 

        public static IDatabase CreateOleDatabase()
        {
            return new OleDatabase(); 
        }

        public static IDatabase CreateOleDatabase(string connectionString)
        {
            IDatabase database = new OleDatabase();
            database.ConnectionString = connectionString;
            return database;
        }

        public static IDatabase CreateSqliteDatabase()
        {
            return new SqliteDatabase();
        }

        public static IDatabase CreateSqliteDatabase(string connectionString)
        {
            IDatabase database = new SqliteDatabase();
            database.ConnectionString = connectionString;
            return database;
        }

        public static IDatabase CreateMySqlDatabase()
        {
            return new MySqlDatabase();
        }

        public static IDatabase CreateMySqlDatabase(string connectionString)
        {
            IDatabase database = new MySqlDatabase();
            database.ConnectionString = connectionString;
            return database;
        }

        public static IDatabase CreateInstance(DatabaseType databaseType)
        {
            IDatabase database = null;
            switch (databaseType)
            {
                case DatabaseType.SqlServer: 
                    database = CreateSqlServerDatabase();
                    break;
                case DatabaseType.Oracle: 
                    database = CreateOracleDatabase();
                    break;
                case DatabaseType.Oledb: 
                    database = CreateOleDatabase();
                    break;
                case DatabaseType.Sqlite:
                    database = CreateSqliteDatabase();
                    break;
                case DatabaseType.MySql:
                    database = CreateMySqlDatabase();
                    break;
                default: 
                    database = CreateSqlServerDatabase();
                    break;
            }
            return database;
        }

        public static IDatabase CreateInstance(DatabaseConfig databaseConfig)
        {
            IDatabase database = CreateInstance(databaseConfig.DatabaseType);
            database.ConnectionString = databaseConfig.ConnectionString;

            return database;
        }
    }
}
