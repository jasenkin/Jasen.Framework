using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jasen.Framework.Core;
using Jasen.Framework.Reflection;

namespace Jasen.Framework.Configuration
{
    public class ConfigManager
    {
        private static readonly ConfigManager Instance = new ConfigManager();

        public static ConfigManager Current
        {
            get
            {
                return Instance;
            }
        }

        public bool AllowMultiple { get; internal set; }

        public DatabaseConfig DefaultConfig { get; set; }

        public IDictionary<Type, DatabaseConfig> Configs
        {
            get;
            private set;
        }

        public ConfigManager()
        {
            this.Configs = new Dictionary<Type, DatabaseConfig>();
        }

        public void Clear()
        {
            this.Configs.Clear();
        }

        public void Register(string sectionName, string configName = null)
        {
            var section = ConfigurationManager.GetSection(sectionName) as DatabaseConfigurationSection;
            string defaultConfigName = section.Settings.Default;

            foreach (SettingConfigurationElement element in section.Settings)
            {
                if(string.IsNullOrWhiteSpace(element.Name)||
                    string.IsNullOrWhiteSpace(element.ProviderName))
                {
                    continue;
                }

                DatabaseType databaseType = AttributeUtility.GetDatabaseType(element.ProviderName);
                if (databaseType == DatabaseType.None)
                {
                    throw new ArgumentException(element.ProviderName);
                }
               
                if(string.IsNullOrWhiteSpace(defaultConfigName))
                {
                    DefaultConfig = new DatabaseConfig(element.ConnectionString, databaseType, element.CommandTimeout);
                    break;
                }

                if(string.Equals(defaultConfigName.Trim(),element.Name.Trim()))
                {
                    DefaultConfig = new DatabaseConfig(element.ConnectionString, databaseType, element.CommandTimeout);
                    break;
                }
            }

            if (DefaultConfig == null)
            {
                throw new ArgumentNullException("Invalid DatabaseConfig");
            }
        }
         
        public DatabaseConfig FindConfig(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (!this.AllowMultiple)
            {
                return this.DefaultConfig;
            }

            if (this.Configs.ContainsKey(type))
            {
                return this.Configs[type];
            }  

            if (!type.IsInherit(typeof(IStoreProcedure))
                || !type.IsInherit(typeof(IViewExecutor<>)))
            {
                return null;
            }

            CreateConfigs(type.Assembly);
             
            if (!this.Configs.ContainsKey(type))
            {
                return null;
            }

            return this.Configs[type];
        }

        private void CreateConfigs(Assembly assembly)
        {
            Type assmblyConfigType = assembly.GetTypes().Where(p=>
                p.IsAssignableFrom(typeof(IAssmblyConfig))).FirstOrDefault();
            DatabaseConfig config = null;

            if (assmblyConfigType == null)
            {
                config = this.DefaultConfig;
            }
            else
            {
                 object instance = Activator.CreateInstance(assmblyConfigType);
                 var assmblyConfig = instance as IAssmblyConfig;
                 if(assmblyConfig!=null)
                 {
                    config = assmblyConfig.Config;
                 } 
            }

            if(config==null)
            {
                throw new ArgumentException("Look at System DefaultConfig and Assembly Config.");
            }
          
            IEnumerable<Type> types = assembly.GetTypes().Where(p=>p.IsInherit(typeof(IStoreProcedure))
                ||p.IsInherit(typeof(IViewExecutor<>)));

            foreach (var type in types)
            {
                if (!this.Configs.ContainsKey(type))
                {
                    this.Configs.Add(type,config);
                }
            }
        }

        public static void Write(string configFileName, DatabaseConfig databaseConfigs, string defaultConfigName)
        {
            if (databaseConfigs == null)
            {
                return;
            }

            Write(configFileName, new List<DatabaseConfig>() { databaseConfigs }, defaultConfigName);
        }

        public static void Write(string configFileName, IList<DatabaseConfig> databaseConfigs, string defaultConfigName)
        {
            var file = new ExeConfigurationFileMap();
            file.ExeConfigFilename = configFileName;

            var section = new DatabaseConfigurationSection();
            section.Settings.Default = defaultConfigName;
            databaseConfigs = databaseConfigs ?? new List<DatabaseConfig>();

            foreach (DatabaseConfig config in databaseConfigs)
            {
                var element = new SettingConfigurationElement();
                element.Name = config.DatabaseType.ToString();
                element.ProviderName = config.DatabaseType.ToString();
                element.ConnectionString = config.ConnectionString;
                element.CommandTimeout = config.CommandTimeout;
                section.Settings.Add(element);
            }

            System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            configuration.Sections.Add("databaseConfiguration", section);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
