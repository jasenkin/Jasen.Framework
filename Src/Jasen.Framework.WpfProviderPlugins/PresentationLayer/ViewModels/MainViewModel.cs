using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.Models;
using System.Collections.ObjectModel;
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.Views;
using Jasen.Framework.SchemaProvider;
using Jasen.Framework.WpfProviderPlugins.Common;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Jasen.Framework.WpfProviderPlugins.Languages;
using System.ComponentModel;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class MainViewModel : ViewModelBase 
    {
        private string _language;
        private IDatabaseProvider _provider;
        private ReadOnlyCollection<OperationNodeViewModel> _firstGenerationChildren;
         
        public MainViewModel()
        {
            this.OpenCommand = new RelayCommand<Window>(Open);
            this.SystemSettingCommand = new RelayCommand<Window>(OpenSystemSettingWindow);
            this.CloseCommand = new RelayCommand(CloseApp);
            this.OpenSystemInfoWindowCommand = new RelayCommand<Window>(OnOpenSystemInfoWindow);
            this.FullScreenCommand = new RelayCommand<Window>(OnFullScreen);
            this.ExitFullScreenCommand = new RelayCommand<Window>(OnExitFullScreen);
            this.ChangeLanguageCommand = new RelayCommand(ChangeLanguage);
            this.Language = SystemResource.Language;
        }

        public string Language
        {
            get
            {
                return this._language;
            }
            set
            {
                this._language = value;
                this.RaisePropertyChanged("Language");
            }
        }

        public ReadOnlyCollection<OperationNodeViewModel> FirstGenerationChildren
        {
            get
            {
                return this._firstGenerationChildren;
            }
            set
            {
                this._firstGenerationChildren = value;
                this.RaisePropertyChanged("FirstGenerationChildren");
            }
        }
        
        public string CodeContent
        {
            get;
            private set;
        }

         public ICommand ChangeLanguageCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> SelectedNodeDoubleClickedCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> ExitFullScreenCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> FullScreenCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> SystemSettingCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> OpenSystemInfoWindowCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> CboChangedCommand
        {
            get;
            private set;
        }

        public RelayCommand<Window> OpenCommand
        {
            get;
            private set;
        }

        public ICommand CloseCommand
        {
            get;
            private set;
        }


        public void ChangeLanguage()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

            string language = "zh-CN";

            if (string.Equals(cultureInfo.Name, "zh-CN"))
            {
                language = "en";
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);

            this.Language = SystemResource.Language;
        }

        public void OnTreeViewItemDoubleClick(OperationNodeViewModel nodeViewModel)
        {
            if (nodeViewModel == null || nodeViewModel.OperationType == OperationType.None)
            {
                return;
            }

            GenerateFileContent(nodeViewModel.Name, nodeViewModel.OperationType);
        }

        private void GenerateFileContent(string name, OperationType operationType, bool generateFile = false, bool openFile = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string templateFilePath;
            SystemSetting setting;
            string error;
            if (!CheckSystemSetting(out setting, out templateFilePath, out error))
            {
                MessageBox.Show(error, "系统配置...");
                return;
            }

            GenerateFile(name, operationType, setting, templateFilePath, generateFile, openFile);
        }

        private void GenerateFile(string name, OperationType operationType, SystemSetting setting,
            string templateFilePath, bool generateFile, bool openFile = false)
        {
            string content = this._provider.Build(name.Trim(), operationType,
                                                  setting.DefaultNameSpace, setting.AddAttribute, templateFilePath);

            this.CodeContent = content;

            this.RaisePropertyChanged("CodeContent");

            if (generateFile)
            {
                string outputDir = setting.OutputDir;

                if (!Directory.Exists(setting.OutputDir))
                {
                    Directory.CreateDirectory(SystemConfig.OutputFilePath);
                    outputDir = SystemConfig.OutputFilePath;
                    SystemSetting.Serialize(SystemConfig.SettingFilePath, setting);
                }

                string outputFileName = outputDir + "\\" + name.Trim() + ".cs";
                File.WriteAllText(outputFileName, content.Replace("\n", "\r\n"), Encoding.GetEncoding("gb2312"));
                Process.Start(outputFileName);
            }
        }

        private bool CheckSystemSetting(out SystemSetting setting, out string templateFilePath, out string error)
        {
            setting = SystemSetting.Deserialize(SystemConfig.SettingFilePath);
            templateFilePath = null;
            error = string.Empty;

            if (setting == null)
            {
                error += "不存在相关系统设置，请设置系统配置!";
                return false;
            }

            templateFilePath = SystemConfig.TemplateFilePath + setting.SelectedTemplate;

            if (!File.Exists(templateFilePath))
            {
                error += "不存在有效的模板文件，请设置系统配置模板文件!";
                return false;

            }

            return true;
        }

        public void CloseApp()
        {
            if (MessageBox.Show("你真的要退出应用程序吗？", "提示", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
            {   
                return;
            }

            App.Current.Shutdown();
        }

        public void Open(Window parentWindow)
        {
            SettingWindow window = new SettingWindow();
            window.Owner = parentWindow;
            window.Topmost = true;
            window.SaveCompleted += new SaveCompletedEventHanlder(OnSaveCompleted);
            window.ShowDialog();
        }

        public void OnExitFullScreen(Window window)
        {
            if (window != null)
            {
                window.ExitFullscreen();
            }
        }

        public void OnFullScreen(Window window)
        {
            if (window != null)
            {
                window.ToFullscreen();
            }
        }

        public void OpenSystemSettingWindow(Window parentWindow)
        {
            SystemSettingWindow window = new SystemSettingWindow();
            window.Owner = parentWindow;
            window.Topmost = true;
            window.ShowDialog();
        }
        
        private void OnSaveCompleted(IDatabaseProvider provider)
        {
            this._provider = provider;
            this._provider.Init();

            OperationNode tableParentNode = CreateParentNode("Tables", OperationType.Table, this._provider.TableNames);
            OperationNode viewParentNode = CreateParentNode("Views", OperationType.View, this._provider.ViewNames);
            OperationNode procedureParentNode = CreateParentNode("Procedures", OperationType.Procedure, this._provider.ProcedureNames);

            OperationNode node = new OperationNode("ParentNode", OperationType.None);
            node.Children.Add(tableParentNode);
            node.Children.Add(viewParentNode);
            node.Children.Add(procedureParentNode);

            TreeViewModel viewModel = new TreeViewModel(node);
            this.FirstGenerationChildren = viewModel.FirstGenerationChildren; 
        }

        private OperationNode CreateParentNode(string parentTitle, OperationType operationType, IList<string> childrenNodes)
        {
            OperationNode parentNode = new OperationNode(parentTitle, OperationType.None);

            if (childrenNodes != null && childrenNodes.Count() > 0)
            {
                foreach (string table in childrenNodes)
                {
                    parentNode.Children.Add(new OperationNode(table, operationType));
                }
            }

            return parentNode;
        }

        public void OnOpenSystemInfoWindow(Window parentWindow)
        {
            SystemInfoWindow window = new SystemInfoWindow();
            window.Owner = parentWindow;
            window.Topmost = true;
            window.ShowDialog();
        }
    }
}
