using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.Attributes;
using System.Reflection;

namespace Jasen.Framework
{
    public class PropertyAssociation
    { 

        public PropertyAssociation(PropertyInfo propertyInfo, AssociationAttribute attribute)
        { 
            this.Property = propertyInfo;
            this.Association = attribute;
        }

        public AssociationAttribute Association { get; set; }

        public PropertyInfo Property { get; set; }
    }
}
