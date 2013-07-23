using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.Serialization;

namespace Jasen.Framework.SchemaProvider
{ 
    [Serializable]
    public class SystemSetting
    {
        public string DefaultNameSpace { get; set; }

        public string OutputDir { get; set; }

        public string SelectedTemplate { get; set; }

        public bool AddAttribute { get; set; }


        public static bool Serialize(string filePath, SystemSetting setting)
        {
            if (setting == null)
            {
                return false;
            }

            return SerializerProviderFactory.Create(SerializerType.Xml).
                Serialize(filePath, setting);
        }

        public static SystemSetting Deserialize(string filePath)
        {
            return SerializerProviderFactory.Create(SerializerType.Xml).
                Deserialize<SystemSetting>(filePath);
        }
    }
}
