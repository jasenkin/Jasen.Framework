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
    /// ThreeWizardMenuControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThreeWizardMenuControl : UserControl
    {
        public event EventHandler Wizard1Click;
        public event EventHandler Wizard2Click;
        public event EventHandler Wizard3Click;

        public ThreeWizardMenuControl()
        {
            InitializeComponent();

            this.OnWizardMenuChangeColorEvents();
        }


        public void SelectMenu1()
        {
            this.Wizard1EnablePath.Visibility = Visibility.Visible;
            this.Wizard2EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard3EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard1Border.Visibility = Visibility.Visible;
            this.Wizard2Border.Visibility = Visibility.Collapsed;
            this.Wizard3Border.Visibility = Visibility.Collapsed;

            if (this.Wizard1Click != null)
            {
                this.Wizard1Click(this.Wizard1Grid, EventArgs.Empty);
            }
        }

        public void SelectMenu2()
        {
            this.Wizard1EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard2EnablePath.Visibility = Visibility.Visible;
            this.Wizard3EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard1Border.Visibility = Visibility.Collapsed;
            this.Wizard2Border.Visibility = Visibility.Visible;
            this.Wizard3Border.Visibility = Visibility.Collapsed;

            if (this.Wizard2Click != null)
            {
                this.Wizard2Click(this.Wizard2Grid, EventArgs.Empty);
            }
        }

        public void SelectMenu3()
        {
            this.Wizard1EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard2EnablePath.Visibility = Visibility.Collapsed;
            this.Wizard3EnablePath.Visibility = Visibility.Visible;
            this.Wizard1Border.Visibility = Visibility.Collapsed;
            this.Wizard2Border.Visibility = Visibility.Collapsed;
            this.Wizard3Border.Visibility = Visibility.Visible;

            if (this.Wizard3Click != null)
            {
                this.Wizard3Click(this.Wizard3Grid, EventArgs.Empty);
            }
        }

        private void OnWizardMenuChangeColorEvents()
        {
            this.Wizard1Grid.MouseEnter += (s, e) => { this.Wizard1Border.Visibility = Visibility.Visible; };
            this.Wizard1Grid.MouseLeave += (s, e) => { this.Wizard1Border.Visibility = this.Wizard1EnablePath.Visibility; };

            this.Wizard2Grid.MouseEnter += (s, e) => { this.Wizard2Border.Visibility = Visibility.Visible; };
            this.Wizard2Grid.MouseLeave += (s, e) => { this.Wizard2Border.Visibility = this.Wizard2EnablePath.Visibility; };

            this.Wizard3Grid.MouseEnter += (s, e) => { this.Wizard3Border.Visibility = Visibility.Visible; };
            this.Wizard3Grid.MouseLeave += (s, e) => { this.Wizard3Border.Visibility = this.Wizard3EnablePath.Visibility; };

            MouseButtonEventHandler wizardSelect = (s, e) =>
            {
                if (s == Wizard1EnablePath || s == Wizard1DisablePath || s == Wizard1Text)
                {
                    if (this.Wizard1EnablePath.Visibility == Visibility.Collapsed)
                        this.SelectMenu1();
                }
                else if (s == Wizard2EnablePath || s == Wizard2DisablePath || s == Wizard2Text)
                {
                    if (this.Wizard2EnablePath.Visibility == Visibility.Collapsed)
                        this.SelectMenu2();
                }
                else if (s == Wizard3EnablePath || s == Wizard3DisablePath || s == Wizard3Text)
                {
                    if (this.Wizard3EnablePath.Visibility == Visibility.Collapsed)
                    {

                    }
                }
            };

            this.Wizard1EnablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard1DisablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard1Text.MouseLeftButtonDown += wizardSelect;

            this.Wizard2EnablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard2DisablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard2Text.MouseLeftButtonDown += wizardSelect;

            this.Wizard3EnablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard3DisablePath.MouseLeftButtonDown += wizardSelect;
            this.Wizard3Text.MouseLeftButtonDown += wizardSelect;
        }

    }
}
