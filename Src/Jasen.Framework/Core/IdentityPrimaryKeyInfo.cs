using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jasen.Framework.Core
{
    public class IdentityPrimaryKeyInfo
    {
        public string Name { get; set; }

        public ColumnAttribute ColumnAttribute { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
