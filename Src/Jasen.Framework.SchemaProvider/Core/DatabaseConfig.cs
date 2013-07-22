using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Jasen.Framework
{
    [Serializable]
    public class DatabaseConfig
    { 
        private int _commandTimeout = 60;

        public string ConnectionString { get; set; }
         
        public int CommandTimeout
        {
            get
            {
                return _commandTimeout;
            }
            set
            {
                if (value <= 0)
                {
                    _commandTimeout = DatabaseConfig.DefaultCommandTimeout;
                    return;
                }
                _commandTimeout = value;
            }
        }
         
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.ConnectionString);
            }
        }
         
        public const int DefaultCommandTimeout = 60;
         
        public DatabaseConfig()
        {
        }
           
        public DatabaseConfig(string connectionString,
            int commandTimeout = DefaultCommandTimeout)
        {
            if (commandTimeout <= 0)
            {
                commandTimeout = DatabaseConfig.DefaultCommandTimeout;
            }

            this.ConnectionString = connectionString; 
            this.CommandTimeout = commandTimeout;
        } 
    }
}
