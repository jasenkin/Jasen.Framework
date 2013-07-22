using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    public class TableColumn
    {
        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int ColumnFlag { get; set; }

        public string DataType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsForeignKey { get; set; }

        public string ReferenceTableName { get; set; }

        public string ReferenceKey { get; set; }
    }
}
