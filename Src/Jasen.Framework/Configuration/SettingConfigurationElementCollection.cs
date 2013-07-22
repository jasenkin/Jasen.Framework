using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Configuration
{
   
    public class SettingConfigurationElementCollection : ConfigurationElementCollection
    {
        public SettingConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as SettingConfigurationElement;
            }

            set
            {
                this.BaseAdd(index, value);
            }
        }

        public int Add(SettingConfigurationElement element)
        {
            if(element==null)
            {
                return -1;
            }

            int index = this.Count;

            this.BaseAdd(index, element);

            return index;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SettingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SettingConfigurationElement)element).Name;
        }

        [ConfigurationProperty("default", IsRequired = true)]
        public string Default
        {
            get { return Convert.ToString(this["default"]); }
            set { this["default"] = value; }
        }

        [ConfigurationProperty("multiple", IsRequired = true,DefaultValue = false)]
        public bool AllowMultiple
        {
            get { return Convert.ToBoolean(this["multiple"]); }
            set { this["multiple"] = value; }
        } 
    }
}
