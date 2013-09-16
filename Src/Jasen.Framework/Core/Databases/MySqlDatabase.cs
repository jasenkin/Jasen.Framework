using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Jasen.Framework.Core.Databases
{ 
    public class MySqlDatabase : Database
	{
        public MySqlDatabase()
		{
            CreateDatabaseObjects();
            StartupDatabaseObjects();
		}

        private void CreateDatabaseObjects()
        {
            base.Connection = new MySqlConnection();
            base.Command = new MySqlCommand();
            base.DataAdapter = new MySqlDataAdapter(); 
        }

        private void StartupDatabaseObjects()
        {
            base.Command.Connection = base.Connection;
            base.DataAdapter.SelectCommand = base.Command;
        }

        public override void ClearAllPools()
        {
            
        }

        public override void ClearPool()
        {
            
        }
    }
}
