using System;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public CommandException Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandOperation Operation
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
