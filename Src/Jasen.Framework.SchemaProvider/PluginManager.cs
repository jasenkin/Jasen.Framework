using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Jasen.Framework.SchemaProvider
{
    public static class PluginManager
    {
        public static IList<ProviderInfo> GetSchemaProviderInfos(string pluginDir)
        {
            IList<IDatabaseProvider> providers = GetSchemaProviders(pluginDir);

            if (providers == null)
            {
                return new List<ProviderInfo>();
            }

            IList<ProviderInfo> providerInfos = new List<ProviderInfo>();
            ProviderInfo providerInfo;

            foreach (var provider in providers)
            {
                providerInfo = new ProviderInfo();
                providerInfo.Name = AttributeUtility.GetProviderName(provider.GetType());
                providerInfo.Provider = provider;
                providerInfos.Add(providerInfo);
            }

            return providerInfos;
        }

        public static IList<IDatabaseProvider> GetSchemaProviders(string pluginDir)
        {
            List<IDatabaseProvider> providers = new List<IDatabaseProvider>();

            if (!Directory.Exists(pluginDir))
            {
                return providers;
            }

            string[] fileNames = Directory.GetFiles(pluginDir);

            if (fileNames.Length <= 0)
            {
                return providers;
            }

            IList<IDatabaseProvider> fileProviders;

            foreach (string fileName in fileNames)
            {
                if (LoadSchemaProviders(fileName, out fileProviders))
                {
                    if (fileProviders != null && fileProviders.Count > 0)
                    {
                        providers.AddRange(fileProviders);
                    }
                }
            }

            return providers;
        }

        public static bool LoadSchemaProviders(string filePath, out IList<IDatabaseProvider> providers)
        {
            providers = new List<IDatabaseProvider>();

            if (!File.Exists(filePath))
            {
                return false;
            }

            if (!filePath.EndsWith(".dll",StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            Type[] types = Assembly.LoadFile(filePath).GetTypes();
            IDatabaseProvider provider;

            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                 
                if (!type.IsSubclassOf(typeof(DatabaseProvider)))
                {
                    continue;
                }

                provider = Activator.CreateInstance(type) as IDatabaseProvider;

                if (provider != null)
                {
                    providers.Add(provider);
                } 
            }

            return true;
        } 
    }
}
