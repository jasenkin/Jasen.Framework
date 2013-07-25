using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.Models;
using GalaSoft.MvvmLight.Command;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class TreeViewModel
    {
        readonly OperationNodeViewModel _rootViewModel;

        public RelayCommand<OperationNodeViewModel> SelectedNodeDoubleClickedCommand
        {
            get;
            set;
        }

        public ReadOnlyCollection<OperationNodeViewModel> FirstGenerationChildren
        {
            get;
            private set;
        }


        public void Changed(OperationNodeViewModel model)
        {

        }

        public TreeViewModel(OperationNode rootNode)
        {
            if (rootNode == null)
            {
                throw new ArgumentNullException("rootNode");
            }
            this.SelectedNodeDoubleClickedCommand = new RelayCommand<OperationNodeViewModel>(this.Changed);

            this._rootViewModel = new OperationNodeViewModel(rootNode);
            this.FirstGenerationChildren = this._rootViewModel.Children;
        }

    }
}
