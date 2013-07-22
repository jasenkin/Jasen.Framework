using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class AssociationAttribute : Attribute
    { 
        public AssociationAttribute()
        {
        }

        public abstract AssociationType AssociationType
        {
            get;
        }

        public Type ElementType { get; set; }

        public Type InterrelationType { get; set; }

        public string InterrelationFilter { get; set; }

        public string ResultFilter { get; set; }

        public Order ResultOrder { get; set; }


    }
}
