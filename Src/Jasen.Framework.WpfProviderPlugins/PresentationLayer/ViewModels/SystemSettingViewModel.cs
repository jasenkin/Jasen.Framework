using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.IO;
using Jasen.Framework.WpfProviderPlugins.Common;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class SystemSettingViewModel : ViewModelBase
    {
        private List<string> _selectedTemplates;

        public string SelectedTemplate
        {
            get;
            set;
        }

        public string OutputDirectory
        {
            get;
            set;
        }

        public bool HasAttribute
        {
            get;
            set;
        }

        public string NameSpace
        {
            get;
            set;
        }

        public List<string> Templates
        {
            get
            {
                return this._selectedTemplates;
            }
            set
            {
                if (this._selectedTemplates != value)
                {
                    this._selectedTemplates = value;
                    this.RaisePropertyChanged("Templates");
                }
            }
        }

        public RelayCommand<Window> SaveCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> CancelCommand
        {
            get;
            private set;
        }

        public SystemSettingViewModel()
        {
            this.SaveCommand = new RelayCommand<Window>(Save);
            this.CancelCommand = new RelayCommand<Window>(Cancel);       
            this.Templates = new List<string>();

            CreateSystemDirectory();
            LoadTemplate();

            LoadDefaultSetting();
        }

        private void LoadDefaultSetting()
        {
            SystemSetting setting = SystemSetting.Deserialize(SystemConfig.SettingFilePath);

            if (setting == null)
            {
                return;
            }

            this.NameSpace = setting.DefaultNameSpace;
            this.OutputDirectory = setting.OutputDir;
            this.HasAttribute = setting.AddAttribute;

            if (this.Templates.Count > 0)
            {
                this.SelectedTemplate = setting.SelectedTemplate;
            }
        }

        private void CreateSystemDirectory()
        {
            if (!Directory.Exists(SystemConfig.TemplateFilePath))
            {
                Directory.CreateDirectory(SystemConfig.TemplateFilePath);
            }

            if (!Directory.Exists(SystemConfig.OutputFilePath))
            {
                Directory.CreateDirectory(SystemConfig.OutputFilePath);
            }

            if (!Directory.Exists(SystemConfig.PluginPath))
            {
                Directory.CreateDirectory(SystemConfig.PluginPath);
            }
        }

        private void LoadTemplate()
        {
            var directoryInfo = new DirectoryInfo(SystemConfig.TemplateFilePath);

            FileInfo[] filePaths = directoryInfo.GetFiles();

            IList<string> fileNames = new List<string>();

            foreach (var filePath in filePaths)
            {
                fileNames.Add(filePath.Name);
            }

            this.Templates.AddRange(fileNames);
        }

        public void Save(Window window)
        {
            if (string.IsNullOrWhiteSpace(this.SelectedTemplate))
            {
                MessageBox.Show("请选择有效的模板!", "系统设置...");

                return;
            }

            if (string.IsNullOrWhiteSpace(this.NameSpace))
            {
                MessageBox.Show("请输入命名空间!", "系统设置...");

                return;
            }

            if (!Directory.Exists(this.OutputDirectory))
            {
                MessageBox.Show("请选择有效的文件输出路径!", "系统设置...");

                return;
            }

            var setting = new SystemSetting();
            setting.AddAttribute = this.HasAttribute;
            setting.DefaultNameSpace = this.NameSpace.Trim();
            setting.OutputDir = this.OutputDirectory;
            setting.SelectedTemplate = this.SelectedTemplate;

            SystemSetting.Serialize(SystemConfig.SettingFilePath, setting);

            if (window != null)
            {
                window.Close();
            }
        }

        public void Cancel(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
