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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jasen.Framework.WpfProviderPlugins.Controls
{
    /// <summary>
    /// WaterMarkTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class WaterMarkTextBox : UserControl
    {
        public WaterMarkTextBox()
        {
            InitializeComponent();

            this.WaterTextBlock.MouseLeftButtonDown += (s, e) =>
            {
                this.WaterTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                this.InputTextBox.Focus();
            };
            this.InputTextBox.LostFocus += (s, e) =>
            {
                if (this.InputTextBox.Text.Length == 0)
                    this.WaterTextBlock.Visibility = System.Windows.Visibility.Visible;
            };
            this.ClearGrid.MouseLeftButtonDown += (s, e) =>
            {
                this.InputTextBox.Text = "";
            };
        }
    }
}
