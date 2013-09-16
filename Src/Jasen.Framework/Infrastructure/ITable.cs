using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Infrastructure
{
    public interface ITable : IView
    {
        List<string> ChangedPropertyNames { get; }

        void ClearChangedPropertyNames();
    }
}
