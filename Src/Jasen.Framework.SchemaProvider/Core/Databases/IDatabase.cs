using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Jasen.Framework
{
    /// <summary>
    /// The interface of database.
    /// </summary>
    [ComVisible(false)]
    public interface IDatabase
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// The state of connection.
        /// </summary>
        ConnectionState ConnectionState
        {
            get;
        }

        /// <summary>
        /// Connection to database.
        /// </summary>
        IDbConnection Connection
        {
            get;
        }

        /// <summary>
        /// DataAdapter
        /// </summary>
        IDbDataAdapter DataAdapter
        {
            get;
        }

        /// <summary>
        /// Command
        /// </summary>
        IDbCommand Command
        {
            get;
        }

        /// <summary>
        /// Transaction
        /// </summary>
        IDbTransaction Transaction
        {
            get;
        }

        /// <summary>
        /// Parameters
        /// </summary>
        IDataParameterCollection Parameters
        {
            get;
        }

  

        /// <summary>
        /// Closes the connection to the database.
        /// When exception occurs,it will clear all connections in pool.
        /// </summary>
        /// <returns></returns>
        bool Close();

        /// <summary>
        /// Connects to a database.
        /// When exception occurs,it will clear all connections in pool.
        /// </summary>
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// Clears all connections in pool.
        /// </summary>
        void ClearAllPools();

        /// <summary>
        /// Clears current connection in pool.
        /// </summary>
        void ClearPool();

        /// <summary>
        /// Connects to another database.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        bool ChangeDatabase(string databaseName);

        /// <summary>
        /// Begins a database transaction .
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
 
        IDataReader ExecuteReader(string commandText, bool isProcedure);
        IDataReader ExecuteReader(string commandText, bool isProcedure,IDataParameter parameter);
        IDataReader ExecuteReader(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        int ExecuteNonQuery(string commandText, bool isProcedure);
        int ExecuteNonQuery(string commandText, bool isProcedure, IDataParameter parameter);
        int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        object ExecuteScalar(string commandText, bool isProcedure);
        object ExecuteScalar(string commandText, bool isProcedure, IDataParameter parameter);
        object ExecuteScalar(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        DataSet ExecuteDataSet(string commandText, bool isProcedure);
        DataSet ExecuteDataSet(string commandText, bool isProcedure, IDataParameter parameter);
        DataSet ExecuteDataSet(string commandText, bool isProcedure, IList<IDataParameter> parameters);
       
        DataTable ExecuteDataTable(string commandText, bool isProcedure);
        DataTable ExecuteDataTable(string commandText, bool isProcedure, IDataParameter parameter);
        DataTable ExecuteDataTable(string commandText, bool isProcedure, IList<IDataParameter> parameters);
       
        ITableCommandExecutor CreateTableCommandExecutor();
        IViewCommandExecutor CreateViewCommandExecutor();

    }
}
