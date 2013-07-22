using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    public class PropertyElement
    {
        public PropertyElement()
        {
            this.PropertyType = PropertyType.Element;
        }

        public Type ElementType { get; set; } 

        public PropertyType PropertyType { get; set; }
    }
}
