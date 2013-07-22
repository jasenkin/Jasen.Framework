using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Jasen.Framework.SqliteSchemaProvider
{
    public class SqliteDatabase : Database
    {
        public SqliteDatabase()
        {
            CreateDatabaseObjects();
        }

        private void CreateDatabaseObjects()
        {
            base.Connection = new SQLiteConnection();
            base.Command = new SQLiteCommand();
            base.DataAdapter = new SQLiteDataAdapter();
            base.Command.Connection = base.Connection;
            base.DataAdapter.SelectCommand = base.Command; 
        }

        public override void ClearAllPools()
        {
            SQLiteConnection.ClearAllPools();
        } 

        public override void ClearPool()
        {
            SQLiteConnection.ClearPool(base.Connection as SQLiteConnection);
        }

    }
}
