using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Configuration
{
  
    public class SettingConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return this["connectionString"].ToString(); }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("providerName", IsRequired = true)]
        public string ProviderName
        {
            get { return this["providerName"].ToString(); }
            set { this["providerName"] = value; }
        }

        [ConfigurationProperty("commandTimeout")]
        public int CommandTimeout
        {
            get
            {
                if (this["commandTimeout"]==null)
                {
                    return -1;
                }

                int timeout;
                int.TryParse(this["commandTimeout"].ToString(), out timeout);
                return timeout;
            }
            set
            {
                this["commandTimeout"] = value;
            }
        }
    }
}
