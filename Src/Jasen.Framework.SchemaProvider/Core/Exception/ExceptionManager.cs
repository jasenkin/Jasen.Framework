using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static event ConnectionHandler OnConnectionException;

        /// <summary>
        /// 
        /// </summary>
        public static event CommandHandler OnCommandException;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal static void RaiseExceptionEvent(IDbConnection sender, ConnectionEventArgs args)
        {
            if (ExceptionManager.OnConnectionException != null)
            {
                ExceptionManager.OnConnectionException(sender, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal static void RaiseExceptionEvent(IDbCommand sender, CommandEventArgs args)
        {
            if (ExceptionManager.OnCommandException != null)
            {
                ExceptionManager.OnCommandException(sender, args);
            }
        }
    }
}
