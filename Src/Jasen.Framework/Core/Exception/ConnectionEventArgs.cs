using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionEventArgs:EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectionException Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionOperation Operation
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ThrowException
        {
            get;
            set;
        }
    }
}
