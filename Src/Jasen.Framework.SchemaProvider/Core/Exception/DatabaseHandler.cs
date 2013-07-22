using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="args"></param>
    public delegate void ConnectionHandler(IDbConnection sender, ConnectionEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="args"></param>
    public delegate void CommandHandler(IDbCommand sender, CommandEventArgs args);

}
