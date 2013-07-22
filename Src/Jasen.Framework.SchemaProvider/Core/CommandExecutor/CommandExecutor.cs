using System;
using System.Data;
using System.Collections.Generic;
using Jasen.Framework.Resources;

namespace Jasen.Framework
{
    public class CommandExecutor : ITableCommandExecutor
    {

        private readonly IDatabase _database;
        private bool _existTransaction;

        #region Properties

        public IDatabase Database
        {
            get
            {
                return _database;
            }
        }

        public bool ExistTransaction
        {
            get
            {
                return _existTransaction;
            }
        }

        #endregion


        public CommandExecutor(IDatabase database)
        {
            if (database == null)
            {
                throw new CommandException(MsgResource.InvalidArguments, "IDatabase database");
            }

            _database = database;
        }

        #region  Transaction

        public IDbTransaction BeginTransaction()
        {
            if (string.IsNullOrEmpty(_database.ConnectionString))
            {
                throw new CommandException(MsgResource.ConnectionStringMissing);
            }

            IDbTransaction transaction = null;

            if (_database.ConnectionState == ConnectionState.Closed)
            { 
                if (_database.Open())
                {
                    transaction = _database.BeginTransaction();
                    _existTransaction = (transaction != null);
                }
                else
                {
                    _existTransaction = false;
                }
            }
            else if (_database.ConnectionState == ConnectionState.Open && !_existTransaction)
            {
                transaction = _database.BeginTransaction();
                _existTransaction = (transaction != null);
            }

            return transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (string.IsNullOrEmpty(_database.ConnectionString))
            {
                throw new CommandException(MsgResource.ConnectionStringMissing);
            }

            IDbTransaction transaction = null;
            if (_database.ConnectionState == ConnectionState.Closed)
            { 
                if (_database.Open())
                {
                    transaction = _database.BeginTransaction(isolationLevel);
                    _existTransaction = (transaction != null);
                }
                else
                {
                    _existTransaction = false;
                }
            }
            else if (_database.ConnectionState == ConnectionState.Open && !_existTransaction)
            {
                transaction = _database.BeginTransaction(isolationLevel);
                _existTransaction = (transaction != null);
            }
            return transaction;

        }

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            if (_database.ConnectionState == ConnectionState.Open)
            {
                bool r = _database.Commit();
                if (r)
                {
                    _existTransaction = false;
                    return _database.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// Rollbacks a transaction.
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            if (_database.ConnectionState == ConnectionState.Open)
            {
                bool r = _database.Rollback();
                if (r)
                {
                    _existTransaction = false;
                    return _database.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// Closes the connection with database.
        /// </summary>
        private void CloseConnection()
        {
            if (!_existTransaction && this._database.ConnectionState == ConnectionState.Open)
            {
                this._database.Close();
            }
        }

        private void OpenConnection()
        {
            if (string.IsNullOrEmpty(_database.ConnectionString))
            {
                throw new ConnectionException(MsgResource.ConnectionStringMissing);
            }
           
            if (this._database.ConnectionState == ConnectionState.Closed)
            {
                this._database.Open();
            }
        }


        #endregion  //Transaction



        #region ExecuteReader



        public IDataReader ExecuteReader(string commandText, bool isProcedure)
        {
            OpenConnection();
            IDataReader reader = _database.ExecuteReader(commandText, isProcedure);

            return reader;
        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure, IDataParameter parameter)
        {
            OpenConnection();
            IDataReader reader = _database.ExecuteReader(commandText, isProcedure, parameter);

            return reader;
        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            OpenConnection();
            IDataReader reader = _database.ExecuteReader(commandText, isProcedure, parameters);

            return reader;
        }


        #endregion


        #region ExecuteNonQuery


        /// <summary>
        /// Executes a SQL statement or procedure ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, bool isProcedure)
        {
            OpenConnection();
            int count = _database.ExecuteNonQuery(commandText, isProcedure);
            CloseConnection();
            return count;
        }

        /// <summary>
        /// Executes a procedure ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameter"></param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, bool isProcedure, IDataParameter parameter)
        {
            OpenConnection();
            int count = _database.ExecuteNonQuery(commandText, isProcedure, parameter);
            CloseConnection();
            return count;
        }

        /// <summary>
        /// Executes a procedure ,and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            OpenConnection();
            int count = _database.ExecuteNonQuery(commandText, isProcedure, parameters);
            CloseConnection();
            return count;
        }

        #endregion


        #region ExecuteScalar

        public object ExecuteScalar(string commandText, bool isProcedure)
        {
            OpenConnection();
            object obj = _database.ExecuteScalar(commandText, isProcedure);
            CloseConnection();
            return obj;
        }

        public object ExecuteScalar(string commandText, bool isProcedure, IDataParameter parameter)
        {
            OpenConnection();
            object obj = _database.ExecuteScalar(commandText, isProcedure, parameter);
            CloseConnection();
            return obj;
        }

        public object ExecuteScalar(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            OpenConnection();
            object obj = _database.ExecuteScalar(commandText, isProcedure, parameters);
            CloseConnection();
            return obj;
        }

        #endregion


        #region ExecuteDataSet

        public DataSet ExecuteDataSet(string commandText, bool isProcedure)
        {
            OpenConnection();
            DataSet ds = _database.ExecuteDataSet(commandText, isProcedure);
            CloseConnection();
            return ds;
        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure, IDataParameter parameter)
        {
            OpenConnection();
            DataSet ds = _database.ExecuteDataSet(commandText, isProcedure, parameter);
            CloseConnection();
            return ds;
        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            OpenConnection();
            DataSet ds = _database.ExecuteDataSet(commandText, isProcedure, parameters);
            CloseConnection();
            return ds;
        }

        #endregion

        #region ExecuteDataTable

        public DataTable ExecuteDataTable(string commandText, bool isProcedure)
        {
            OpenConnection();
            DataTable dt = _database.ExecuteDataTable(commandText, isProcedure);
            CloseConnection();
            return dt;
        }

        public DataTable ExecuteDataTable(string commandText, bool isProcedure, IDataParameter parameter)
        {
            OpenConnection();
            DataTable dt = _database.ExecuteDataTable(commandText, isProcedure, parameter);
            CloseConnection();
            return dt;
        }

        public DataTable ExecuteDataTable(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            OpenConnection();
            DataTable dt = _database.ExecuteDataTable(commandText, isProcedure, parameters);
            CloseConnection();
            return dt;
        }

    
        #endregion

        public override string ToString()
        {
            return _database.Command.CommandText;
        }
    }
}
