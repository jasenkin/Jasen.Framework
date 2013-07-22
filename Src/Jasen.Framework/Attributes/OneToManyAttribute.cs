using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework.Attributes
{    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class OneToManyAttribute : AssociationAttribute
    { 
        public OneToManyAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override AssociationType AssociationType
        {
            get
            {
                return AssociationType.OneToMany;
            }
        }


        private new Type InterrelationType { get; set; }

        private new string InterrelationFilter { get; set; }
        
    }
}
