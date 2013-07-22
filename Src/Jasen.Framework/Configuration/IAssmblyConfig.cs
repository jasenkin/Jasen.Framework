using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Configuration
{
    public interface IAssmblyConfig
    {
        DatabaseConfig Config { get; }
    }
}
