using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    public class SqlServer2000Provider : SqlServerProvider
    {
         private ISqlGenerator _sqlBuilder;  

        public override ISqlGenerator SqlBuilder 
        { 
            get 
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new SqlServer2000SqlGenerator();
                }

                return this._sqlBuilder;
            } 
        }

        public SqlServer2000Provider()
        { 
        
        }
    }
}
