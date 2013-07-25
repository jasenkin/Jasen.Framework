using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Jasen.Framework.WpfProviderPlugins.PresentationLayer.Models;
using System.Collections.ObjectModel;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class OperationNodeViewModel : ViewModelBase
    {
        private readonly ReadOnlyCollection<OperationNodeViewModel> _children;
        private readonly OperationNodeViewModel _parent;
        private readonly OperationNode _node;
        private bool _isExpanded;
        private bool _isSelected;

        public OperationNodeViewModel(OperationNode person)
            : this(person, null)
        {
        }

        private OperationNodeViewModel(OperationNode node, OperationNodeViewModel parent)
        {
            this._node = node;
            this._parent = parent;

            this._children = new ReadOnlyCollection<OperationNodeViewModel>(
                    (from child in _node.Children
                     select new OperationNodeViewModel(child, this))
                     .ToList<OperationNodeViewModel>());
        }

        public ReadOnlyCollection<OperationNodeViewModel> Children
        {
            get
            {
                return this._children; 
            }
        }

        public string Name
        {
            get 
            {
                if (this._node != null)
                {
                    return this._node.Name;
                }

                return string.Empty;
            }
        }

        public OperationType OperationType
        {
            get
            {
                if (this._node != null)
                {
                    return this._node.OperationType;
                }

                return SchemaProvider.OperationType.None;
            }
        }

        public bool IsExpanded
        {
            get 
            { 
                return _isExpanded; 
            }
            set
            {
                if (value != this._isExpanded)
                {
                    this._isExpanded = value;
                    this.RaisePropertyChanged("IsExpanded");
                }

                if (this._isExpanded && this._parent != null)
                {
                    this._parent.IsExpanded = true; 
                }
            }
        }

        public bool IsSelected
        {
            get 
            { 
                return this._isSelected;
            }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        public OperationNodeViewModel Parent
        {
            get 
            { 
                return this._parent;
            }
        }

    }
}
