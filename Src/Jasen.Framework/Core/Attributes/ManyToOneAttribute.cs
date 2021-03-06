﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework.Attributes
{   
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ManyToOneAttribute : AssociationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ManyToOneAttribute()
        {
        }
         
        public override AssociationType AssociationType
        {
            get
            {
                return AssociationType.ManyToOne;
            }
        } 

        private new Type InterrelationType { get; set; }

        private new string InterrelationFilter { get; set; }

        private new string ResultFilter { get; set; }

        private new string ResultOrder { get; set; }
    }
}
