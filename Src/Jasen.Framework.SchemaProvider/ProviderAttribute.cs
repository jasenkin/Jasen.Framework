using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.SchemaProvider
{ 
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ProviderAttribute : Attribute
    {
        public ProviderAttribute()
        {
        } 

        public ProviderAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
