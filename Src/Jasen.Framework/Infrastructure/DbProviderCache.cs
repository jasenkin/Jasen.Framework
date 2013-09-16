using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jasen.Framework
{
    public class DbProviderCache
    {
        public const int COMMAND_TIMEOUT = 60;

        private static DbProviderCache _contextInstance = null;
        private static readonly object _lockedObj = new object();
        private Dictionary<string, DatabaseProvider> _databaseStrategies = new Dictionary<string, DatabaseProvider>();

        private DbProviderCache()
        { 
        }

        public static DbProviderCache Current
        {
            get
            {
                if (_contextInstance == null)
                {
                    lock (_lockedObj)
                    {
                        if (_contextInstance == null)
                        {
                            _contextInstance = new DbProviderCache();
                        }
                    }
                }

                return _contextInstance;
            }
        }

        public void AddProviderConfiguration(string name, int commandTimeout = 60)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name.Trim()];

            if (settings == null)
            {
                throw new ArgumentNullException(string.Format("Current configuration has no ConnectionStringSettings of {0}", name));
            }

            AddProviderConfiguration(settings.Name, settings.ConnectionString, settings.ProviderName, commandTimeout);
        }

        public void AddProviderConfiguration(string name, string connectionString, string providerName, int commandTimeout = 60)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (commandTimeout < 0)
            {
                throw new ArgumentOutOfRangeException("commandTimeout");
            }

            name = name.Trim();
            connectionString = connectionString.Trim();
            providerName = (providerName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(providerName))
            {
                DatabaseProvider strategy = new SqlServerProvider();
                strategy.Database.ConnectionString = connectionString;
                strategy.Database.Command.CommandTimeout = commandTimeout;

                this._databaseStrategies.Add(name, strategy);
            }
            else
            {
                if (!providerName.Contains(','))
                {
                    providerName += "," + typeof(DbProviderCache).Assembly.FullName;
                }

                DatabaseProvider strategy = TypeExtension.CreateInstance<DatabaseProvider>(providerName);
                strategy.Database.ConnectionString = connectionString;
                strategy.Database.Command.CommandTimeout = commandTimeout < 0 ? COMMAND_TIMEOUT : commandTimeout;

                this._databaseStrategies.Add(name, strategy);
            }
        }

        public void RemoveProviderConfiguration(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            name = name.Trim();

            if (!this._databaseStrategies.ContainsKey(name))
            {
                return;
            }

            this._databaseStrategies.Remove(name);
        }

        public DatabaseProvider this[string name]
        {
            get
            { 
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }

                if (!this._databaseStrategies.ContainsKey(name.Trim()))
                {
                    throw new ArgumentException(string.Format("Current configuration has no ConnectionStringSettings of {0}", name));
                }

                return this._databaseStrategies[name];
            }
        }

        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return this._databaseStrategies.ContainsKey(name);
        }

        public DatabaseProvider this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                } 

                return this._databaseStrategies.Values.First();
            }
        }

        public int Count
        {
            get
            {
                return this._databaseStrategies.Count;
            }
        }
    }
}
