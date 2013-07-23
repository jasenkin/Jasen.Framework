using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Diagnostics;
using System.Windows;

namespace Jasen.Framework.WpfProviderPlugins.Common
{
    public class SystemConfig
    {
        public static readonly string StartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static readonly string PluginPath = StartupPath + @"\Plugins\";
        public static readonly string SourceFilePath = StartupPath + @"\DataSource.xml";
        public static readonly string TemplateFilePath = StartupPath + @"\Template\";
        public static readonly string SettingFilePath = StartupPath + @"\Setting.xml";
        public static readonly string OutputFilePath = StartupPath + @"\Output\";
    }
}
