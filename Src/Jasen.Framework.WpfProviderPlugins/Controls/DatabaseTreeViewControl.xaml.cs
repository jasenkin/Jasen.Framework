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
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.WpfProviderPlugins.Controls
{
    public delegate void TreeViewItemDoubleClickEventHandler(OperationNodeViewModel nodeViewModel);

    /// <summary>
    /// DatabaseTreeViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class DatabaseTreeViewControl : UserControl
    {
        public event TreeViewItemDoubleClickEventHandler TreeViewItemDoubleClick;

        public DatabaseTreeViewControl()
        {
            InitializeComponent();
        }

        public void OnItemMouseDoubleClick(object  sender, object arg)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;

            if (treeViewItem == null)
            {
                return;
            }

            var viewModel = treeViewItem.Header as OperationNodeViewModel;

            if (viewModel != null && viewModel.OperationType != OperationType.None)
            {
                if (this.TreeViewItemDoubleClick != null)
                {
                    this.TreeViewItemDoubleClick(viewModel);
                }
            }
        }

    }
}
