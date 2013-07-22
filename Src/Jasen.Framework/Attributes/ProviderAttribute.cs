using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Attributes
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ProviderAttribute : Attribute
    {
        public ProviderAttribute()
        {
        }

        public ProviderAttribute(params string[] providerName)
        {
            this.Name = providerName;
        }
         
        public string[] Name { get; set; }

    }
}