using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.Views
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public event SaveCompletedEventHanlder SaveCompleted;

        public SettingWindow()
        {
            InitializeComponent();

            var viewModel = this.DataContext as SettingViewModel;

            if (viewModel != null)
            {
                viewModel.SaveCompleted += new SaveCompletedEventHanlder(viewModel_SaveCompleted);
            }
        }

        private void viewModel_SaveCompleted(IDatabaseProvider config)
        {
            if (this.SaveCompleted != null)
            {
                this.SaveCompleted(config);
            }
        }
    }
}
