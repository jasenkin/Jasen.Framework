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

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
      
    public class SettingViewModel : ViewModelBase
    {
        public SettingViewModel()
        {
            
            this.SaveCommand = new RelayCommand(Save);
            this.CancelCommand = new RelayCommand(Cancel); 
            this.ProviderChangedCommand = new RelayCommand<object>(new Action<object>(this.ProviderSelectedItemChanged));
            this.ConnectionNameChangedCommand = new RelayCommand<object>(new Action<object>(this.ConnectionNameSelectedItemChanged));

            this.Providers = new List<ProviderInfo>();
            this.DataSources = new List<DataSource>();

            LoadProviderPlugins();
            BindSources();
        }

        private void BindSources()
        {
            var providers = PluginManager.GetSchemaProviderInfos(SystemConfig.PluginPath) ?? new List<ProviderInfo>();
             
            this.Providers = providers.ToList();
        }

        private void LoadProviderPlugins()
        {
            DataSourceSetting setting = DataSourceSetting.Deserialize(SystemConfig.SourceFilePath);

            if (setting == null || setting.DataSources == null || setting.DataSources.Count == 0)
            {
                return;
            }
             
            this.DataSources = setting.DataSources;
        }

        private ProviderInfo _selectedProvider;
        private List<ProviderInfo> _providers;
        private DataSource _selectedDataSource;
        private List<DataSource> _dataSources;

        public ProviderInfo SelectedProvider
        {
            get { return _selectedProvider; }
            set { _selectedProvider = value; }
        }


        public List<ProviderInfo> Providers
        {
            get { return this._providers; }
            set
            {
                if (this._providers != value)
                {
                    this._providers = value;
                    this.RaisePropertyChanged("Providers");
                }
            }
        }

        public DataSource SelectedDataSource
        {
            get { return _selectedDataSource; }
            set { _selectedDataSource = value; }
        }

        public List<DataSource> DataSources
        {
            get { return this._dataSources; }
            set
            {
                if (this._dataSources != value)
                {
                    this._dataSources = value;
                    this.RaisePropertyChanged("DataSources");
                }
            }
        }

        public ICommand SaveCommand
        {
            get;
            private set;
        }

        public ICommand CancelCommand
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

        public void Save()
        { 
        
        }

        public void Cancel()
        {

        }

        public void ProviderSelectedItemChanged(object data)
        {
          
        }

        public void ConnectionNameSelectedItemChanged(object data)
        {
             
        }     
    }
}
