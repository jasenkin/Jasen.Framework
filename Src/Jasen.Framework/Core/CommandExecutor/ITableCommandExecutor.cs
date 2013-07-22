using System;
using System.Data;
using System.Collections.Generic;

namespace Jasen.Framework
{
  
    public interface ITableCommandExecutor : IViewCommandExecutor
    {
        bool ExistTransaction
        {
            get;
        }

        IDbTransaction BeginTransaction();

        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
 
        bool Commit();

        bool Rollback();

        int ExecuteNonQuery(string commandText, bool isProcedure, IDataParameter parameter);

        int ExecuteNonQuery(string commandText, bool isProcedure);

        int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters);
    }
}
