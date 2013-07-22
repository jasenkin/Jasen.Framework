using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Jasen.Framework
{
	/// <summary>
    /// Represents the exception that raises when connecting to database.
	/// </summary>
    [Serializable]
    public class ConnectionException : Exception
    {
         /// <summary>
        /// 
        /// </summary>
        public ConnectionException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConnectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public ConnectionException(string message,Exception ex)
            : base(message, ex)
        {
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Message;
        }

    }

}
