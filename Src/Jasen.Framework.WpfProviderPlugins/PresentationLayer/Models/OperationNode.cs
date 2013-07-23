using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.Models
{
    public class OperationNode 
    {
        public OperationNode(string name)
            : this(name, OperationType.Table)
        {

        }

        public OperationNode(string name, OperationType operationType)
        {
            this.Name = name;
            this.OperationType = operationType;
            this.Children = new List<OperationNode>();
        }

        public IList<OperationNode> Children
        {
            get;
            set;
        }

        public OperationType OperationType { get; set; }

        public string Name { get; set; }
    }
}
