using System.Reflection;
using System.IO;

namespace System.Waf.Applications
{ 
    public static class ApplicationInfo
    {
        private static string _productName;
        private static bool _productNameCached;
        private static string _version;
        private static bool _versionCached;
        private static string _company;
        private static bool _companyCached;
        private static string _copyright;
        private static bool _copyrightCached;
        private static string _applicationPath;
        private static bool _applicationPathCached;

         
        public static string ProductName
        {
            get
            {
                if (!_productNameCached)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        AssemblyProductAttribute attribute = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(
                            entryAssembly, typeof(AssemblyProductAttribute)));
                        _productName = (attribute != null) ? attribute.Product : "";
                    }
                    else
                    {
                        _productName = "";
                    }

                    _productNameCached = true;
                }
                return _productName;
            }
        }
         
        public static string Version
        {
            get
            {
                if (!_versionCached)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        _version = entryAssembly.GetName().Version.ToString();
                    }
                    else
                    {
                        _version = "";
                    }
                    _versionCached = true;
                }
                return _version;
            }
        }
         
        public static string Company
        {
            get
            {
                if (!_companyCached)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        AssemblyCompanyAttribute attribute = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                            entryAssembly, typeof(AssemblyCompanyAttribute)));
                        _company = (attribute != null) ? attribute.Company : "";
                    }
                    else
                    {
                        _company = "";
                    }
                    _companyCached = true;
                }
                return _company;
            }
        }
         
        public static string Copyright
        {
            get
            {
                if (!_copyrightCached)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        AssemblyCopyrightAttribute attribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                            entryAssembly, typeof(AssemblyCopyrightAttribute));
                        _copyright = attribute != null ? attribute.Copyright : "";
                    }
                    else
                    {
                        _copyright = "";
                    }
                    _copyrightCached = true;
                }
                return _copyright;
            }
        }
         
        public static string ApplicationPath
        {
            get
            {
                if (!_applicationPathCached)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        _applicationPath = Path.GetDirectoryName(entryAssembly.Location);
                    }
                    else
                    {
                        _applicationPath = "";
                    }
                    _applicationPathCached = true;
                }
                return _applicationPath;
            }
        }
    }
}
