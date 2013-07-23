using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.Serialization;

namespace Jasen.Framework.SchemaProvider
{ 
    [Serializable]
    public class DataSource 
    {
        public string Name
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        } 
    }

    [Serializable]
    public class DataSourceSetting
    {
        public DataSourceSetting()
        {
            this.DataSources = new List<DataSource>();
        }
        public string SelectedDataSource
        {
            get;
            set;
        }

        public List<DataSource> DataSources
        {
            get;
            set;
        }


        public static bool Serialize(string filePath, DataSourceSetting setting)
        {
            if (setting == null)
            {
                return false;
            }

            return SerializerProviderFactory.Create(SerializerType.Xml).
                Serialize(filePath, setting);
        }

        public static DataSourceSetting Deserialize(string filePath)
        {
            return SerializerProviderFactory.Create(SerializerType.Xml).
                Deserialize<DataSourceSetting>(filePath);
        }
    }
}
