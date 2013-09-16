using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.MetaData
{
    
    public class PrimaryKey
    {
        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("FKEY_FROM_COLUMN")]
        public string Key { get; set; }
    }
}
