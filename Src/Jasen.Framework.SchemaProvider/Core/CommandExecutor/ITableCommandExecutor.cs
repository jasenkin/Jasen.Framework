using System;
using System.Data;
using System.Collections.Generic;

namespace Jasen.Framework
{
    /// <summary>
    /// The interface of transation with database tables.
    /// </summary>
    public interface ITableCommandExecutor : IViewCommandExecutor
    {
        /// <summary>
        /// Indicats whether there is a database transaction.
        /// </summary>
        bool ExistTransaction
        {
            get;
        }

        /// <summary>
        /// Begins a database transaction with 60s active time.
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Begins a database transaction with specified IsolationLevel value.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
 
        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <returns></returns>
        bool Commit();

        /// <summary>
        /// Rolls the database transaction.
        /// </summary>
        /// <returns></returns>
        bool Rollback();

        /// <summary>
        /// Executes a procedure or sql statement ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, bool isProcedure, IDataParameter parameter);

        /// <summary>
        /// Executes a procedure or sql statement ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, bool isProcedure);

        /// <summary>
        /// Executes a procedure or sql statement ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters);
    }
}
