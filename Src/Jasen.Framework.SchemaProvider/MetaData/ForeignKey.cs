using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.MetaData
{
    
    public class ForeignKey
    { 
        public string TableName { get; set; }
         
        public string Key { get; set; }
         
        public string ReferenceTableName { get; set; }
         
        public string ReferenceKey { get; set; }
    }
}
