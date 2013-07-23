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

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class MainViewModel : ViewModelBase 
    {
        public MainViewModel()
        {
            this.OpenCommand = new RelayCommand(Open);
            this.CloseCommand = new RelayCommand(CloseApp);
            this.OpenSystemInfoWindowCommand = new RelayCommand(OnOpenSystemInfoWindow);
            this.CboChangedCommand = new RelayCommand<object>(new Action<object>(this.SelectedItemChanged));
            this.ObservComboData = CreateData();
        }

        private ComboData _selectedTemplate;
        private ObservableCollection<ComboData> _observComboData;

        public ComboData SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; }
        }

      
        public ObservableCollection<ComboData> ObservComboData
        {
            get { return this._observComboData; }
            set
            {
                if (this._observComboData != value)
                {
                    this._observComboData = value;
                    this.RaisePropertyChanged("ObservComboData");
                }
            }
        }


        public ICommand OpenSystemInfoWindowCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> CboChangedCommand
        {
            get;
            private set;
        }

        public ICommand OpenCommand
        {
            get;
            private set;
        }

        public ICommand CloseCommand
        {
            get;
            private set;
        }

        public ObservableCollection<ComboData> CreateData()
        {
            ObservableCollection<ComboData> comboData = new ObservableCollection<ComboData>();
            comboData.Add(new ComboData("Bert O'Neill", 300));
            comboData.Add(new ComboData("Tiernan O'Neill", 400));
            comboData.Add(new ComboData("Shannine O'Neill", 500));
            comboData.Add(new ComboData("Cathy O'Neill", 600));

            return comboData;
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

        public void Open()
        {
            SettingWindow window = new SettingWindow();
            window.ShowDialog();
        }

        public void OnOpenSystemInfoWindow()
        {
            SystemInfoWindow window = new SystemInfoWindow();
            window.ShowDialog();
        }

        public void SelectedItemChanged(object data)
        {
            if (data == null || data.GetType() != typeof(ComboData))
            {
                return;
            }

            var currentData = data as ComboData;

            MessageBox.Show(currentData.Data, "提示");
        }     

    }
}
