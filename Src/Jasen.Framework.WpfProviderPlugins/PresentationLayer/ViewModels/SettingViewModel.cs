using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Jasen.Framework.SchemaProvider;
using Jasen.Framework.WpfProviderPlugins.Common;
using System.Windows;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public delegate void SaveCompletedEventHanlder(IDatabaseProvider config);

    public class SettingViewModel : ViewModelBase
    {
        private ProviderInfo _selectedProvider;
        private List<ProviderInfo> _providers;
        private DataSource _selectedDataSource;
        private List<DataSource> _dataSources;
        private string _connectionString;

        public event SaveCompletedEventHanlder SaveCompleted;

        public ProviderInfo SelectedProvider
        {
            get
            {
                return this._selectedProvider;
            }
            set
            {
                this._selectedProvider = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this._connectionString = value;
                this.RaisePropertyChanged("ConnectionString");
            }
        }

        public List<ProviderInfo> Providers
        {
            get
            {
                return this._providers;
            }
            set
            {
                if (this._providers != value)
                {
                    this._providers = value;
                    this.RaisePropertyChanged("Providers");
                }
            }
        }

        public string SelectedDataSourceText
        {
            get;
            set;
        }

        public DataSource SelectedDataSource
        {
            get
            {
                return this._selectedDataSource;
            }
            set
            {
                this._selectedDataSource = value;
            }
        }

        public List<DataSource> DataSources
        {
            get
            {
                return this._dataSources;
            }
            set
            {
                if (this._dataSources != value)
                {
                    this._dataSources = value;
                    this.RaisePropertyChanged("DataSources");
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

        public RelayCommand<object> ProviderChangedCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> ConnectionNameChangedCommand
        {
            get;
            private set;
        }

        public SettingViewModel()
        { 
            this.SaveCommand = new RelayCommand<Window>(Save);
            this.CancelCommand = new RelayCommand<Window>(Cancel); 
            this.ProviderChangedCommand = new RelayCommand<object>(new Action<object>(this.OnProviderSelectedItemChanged));
            this.ConnectionNameChangedCommand = new RelayCommand<object>(new Action<object>(this.OnConnectionNameSelectedItemChanged));

            this.Providers = new List<ProviderInfo>();
            this.DataSources = new List<DataSource>();
            
            LoadProviderPlugins();
            BindSources();

            if (this.SelectedDataSource != null)
            {
                this.SelectedDataSourceText = this.SelectedDataSource.Name;
            }
        }

        private void BindSources()
        {
            DataSourceSetting setting = DataSourceSetting.Deserialize(SystemConfig.SourceFilePath);

            if (setting == null || setting.DataSources == null || setting.DataSources.Count == 0)
            {
                return;
            }

            this.DataSources = setting.DataSources;

            if (this.DataSources != null && this.DataSources.Count > 0)
            {
                this.ConnectionString = this.DataSources[0].ConnectionString;
            }          
        }

        private void LoadProviderPlugins()
        {
            var providers = PluginManager.GetSchemaProviderInfos(SystemConfig.PluginPath) ?? new List<ProviderInfo>();

            this.Providers = providers.ToList();
        }

        public void Save(Window window)
        {

            if (this.SelectedProvider == null)
            {
                MessageBox.Show("请选择相关提供者插件。");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                MessageBox.Show("请输入连接字符串。");
                return;
            }

            var providerInfo = this.SelectedProvider as ProviderInfo;
            bool connectSuccess = CheckConnection(ref providerInfo);

            if (!connectSuccess)
            {
                return;
            }

            if (this.SaveCompleted != null)
            {
                this.SaveCompleted(providerInfo.Provider);
            }

            SaveSetting();

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

        public void OnProviderSelectedItemChanged(object data)
        {
            ProviderInfo provider = data as ProviderInfo;

            if (this.SelectedProvider == null)
            {
                
                MessageBox.Show("请选择相关提供者插件。");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                MessageBox.Show("请输入连接字符串。");
                return;
            }

            var providerInfo = this.SelectedProvider as ProviderInfo;
            bool connectSuccess = CheckConnection(ref providerInfo);

            if (!connectSuccess)
            {
                return;
            }

            if (this.SaveCompleted != null)
            {
                this.SaveCompleted(providerInfo.Provider);
            }

            SaveSetting();
             
        }

        private bool CheckConnection(ref ProviderInfo providerInfo)
        {
            bool connectSuccess = false;

            try
            {
                providerInfo.Provider.Database.ConnectionString = this.ConnectionString;
                connectSuccess = providerInfo.Provider.Database.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接不成功：" + ex.Message);
                providerInfo = null;
                connectSuccess = false;
            }
            finally
            {
                if (providerInfo != null && connectSuccess)
                {
                    providerInfo.Provider.Database.Close();
                }
            }

            return connectSuccess;
        }

        private void SaveSetting()
        {
            DataSourceSetting setting = DataSourceSetting.Deserialize(SystemConfig.SourceFilePath) ?? new DataSourceSetting();

            for (int i = 0; i < setting.DataSources.Count; i++)
            {
                if (string.Equals(setting.DataSources[i].Name, this.SelectedDataSourceText))
                {
                    setting.SelectedDataSource = this.SelectedDataSourceText;
                    setting.DataSources[i].ConnectionString = this.ConnectionString;
                    DataSourceSetting.Serialize(SystemConfig.SourceFilePath, setting);
                    return;
                }
            }

            DataSource dataSource = new DataSource();
            dataSource.Name = this.SelectedDataSourceText;
            dataSource.ConnectionString = this.ConnectionString;
            setting.SelectedDataSource = dataSource.Name;
            setting.DataSources.Add(dataSource);

            DataSourceSetting.Serialize(SystemConfig.SourceFilePath, setting);
        }

        public void OnConnectionNameSelectedItemChanged(object data)
        {
            if (data == null)
            {
                return;
            }

            var dataSource = data as DataSource;

            if (dataSource == null)
            {
                return;
            }

            this.ConnectionString = dataSource.ConnectionString;
        }     
    }
}
