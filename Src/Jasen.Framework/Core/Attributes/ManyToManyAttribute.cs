using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework.Attributes
{ 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ManyToManyAttribute : AssociationAttribute
    { 
        public ManyToManyAttribute(Type associationType)
        {
            base.InterrelationType = associationType;
        }
         
        public override AssociationType AssociationType
        {
            get
            {
                return AssociationType.ManyToMany;
            }
        }
    }
}
