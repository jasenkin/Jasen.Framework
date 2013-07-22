using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Configuration
{
    public class AssemblyConfig : IAssmblyConfig
    {
        private readonly  DatabaseConfig _config;

        public AssemblyConfig()
        {
            
        }

        public AssemblyConfig(DatabaseConfig databaseConfig)
        { 
            this._config = databaseConfig;
        }

        public virtual DatabaseConfig Config
        {
            get { return this._config; }
        }
    }
}
