using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Configuration
{ 
    public class DatabaseConfigurationSection : ConfigurationSection
    {
        public DatabaseConfigurationSection() 
        {
        }

        [ConfigurationProperty("settings")]
        public SettingConfigurationElementCollection Settings
        {
            get { return (SettingConfigurationElementCollection)(this["settings"]); }
            set { this["settings"] = value; }
        }
    }
}
