using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ProcedureAttribute : Attribute
    { 
        public ProcedureAttribute()
        {
        }
         
        public ProcedureAttribute(string name)
        {
            this.Name = name;
        }
         
        public string Name { get; set; }

    }
}
