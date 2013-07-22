using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Jasen.Framework
{
    public class SqlCommandParameter
    {
        public SqlCommandParameter()
            :this(string.Empty)
        {
        }

        public SqlCommandParameter(string sql)
        {
           this.Sql = sql;
           this.Parameters = new List<IDataParameter>();    
        }

        public string Sql { get; set; }

        public IList<IDataParameter> Parameters { get; private set; }
    }
}
