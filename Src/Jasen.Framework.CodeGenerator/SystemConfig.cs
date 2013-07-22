using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jasen.Framework.CodeGenerator
{
    public class SystemConfig
    {
        public static readonly string PluginPath = Application.StartupPath + @"\Plugins\";
        public static readonly string SourceFilePath = Application.StartupPath + @"\DataSource.xml";
        public static readonly string TemplateFilePath = Application.StartupPath + @"\Template\";
        public static readonly string SettingFilePath = Application.StartupPath + @"\Setting.xml";
        public static readonly string OutputFilePath = Application.StartupPath + @"\Output\";
    }
}
