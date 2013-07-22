using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.CodeGenerator
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
        }

        public OperationType OperationType { get; set; }

        public string Name { get; set; }
    }
}
