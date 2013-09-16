using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.ConfigurationTestConsoleApp
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext()
            : base("Oracle_JK_LAB")
        { 
         
        }
    }
}
