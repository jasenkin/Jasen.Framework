using System;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

namespace Jasen.Framework.AccessSchemaProvider
{ 
    public class OleDatabase : Database
	{
        public OleDatabase()
		{
            CreateDatabaseObjects();
		}

        private void CreateDatabaseObjects()
        {
            base.Connection = new  OleDbConnection();
            base.Command = new OleDbCommand();
            base.DataAdapter = new OleDbDataAdapter();
            base.Command.Connection = base.Connection;
            base.DataAdapter.SelectCommand = base.Command;
        } 

        public override void ClearAllPools()
        {
            OleDbConnection.ReleaseObjectPool();
        }

        public override void ClearPool()
        {
            OleDbConnection.ReleaseObjectPool();
        }
    }
}
