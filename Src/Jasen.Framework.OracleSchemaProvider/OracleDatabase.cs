using System;
using System.Data;
using System.Data.OracleClient;
using System.Reflection;

namespace Jasen.Framework.OracleSchemaProvider
{ 
    internal class OracleDatabase : Database
	{
        public OracleDatabase()
		{
            CreateDatabaseObjects(); 
		}

        private void CreateDatabaseObjects()
        {
            base.Connection = new  OracleConnection();
            base.Command = new OracleCommand();
            base.DataAdapter = new OracleDataAdapter();
            base.Command.Connection = base.Connection;
            base.DataAdapter.SelectCommand = base.Command;
        } 

        public override void ClearAllPools()
        {
            OracleConnection.ClearAllPools();
        }

        public override void ClearPool()
        {
            OracleConnection.ClearPool(base.Connection as OracleConnection);
        }
    }
}
