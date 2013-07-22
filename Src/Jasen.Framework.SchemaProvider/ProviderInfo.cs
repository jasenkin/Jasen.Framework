using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.SchemaProvider
{
    public class ProviderInfo
    {
        public string Name
        {
            get;
            set;
        }

        public IDatabaseProvider Provider
        {
            get;
            set;
        }
    }
}
