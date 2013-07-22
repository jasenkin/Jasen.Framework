using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Jasen.Framework.SqlServer2000SchemaProvider
{ 
    public class SqlServerDatabase : Database
	{
        public SqlServerDatabase()
		{
            CreateDatabaseObjects(); 
		}

        private void CreateDatabaseObjects()
        {
            base.Connection = new SqlConnection();
            base.Command = new SqlCommand();
            base.DataAdapter = new SqlDataAdapter();
            base.Command.Connection = base.Connection;
            base.DataAdapter.SelectCommand = base.Command;
        } 

        public override void ClearAllPools()
        {
            SqlConnection.ClearAllPools();
        }

        public override void ClearPool()
        {
            SqlConnection.ClearPool(base.Connection as SqlConnection);
        }
    }
}
