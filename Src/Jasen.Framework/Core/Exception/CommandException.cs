using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Jasen.Framework
{
	/// <summary>
    /// Represents the exception that raises when executing a command.
	/// </summary>
    [Serializable]
    public class CommandException : Exception
    {

        private string _commandText;


        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CommandException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandText"></param>
        public CommandException(string message,string commandText)
            : base(message)
        {
            _commandText = commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public CommandException(string message,Exception ex)
            : base(message, ex)
        {
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandText"></param>
        /// <param name="ex"></param>
        public CommandException(string message, string commandText, Exception ex)
            : base(message, ex)
        {
            _commandText = commandText;
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
