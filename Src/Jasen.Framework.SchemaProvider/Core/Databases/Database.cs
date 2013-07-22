using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;
using System.Text;
using Jasen.Framework.Resources;

namespace Jasen.Framework
{
    public abstract class Database : MarshalByRefObject, IDatabase, IDisposable
    {

        private IDbConnection _connection;
        private IDbDataAdapter _dataAdapter;
        private IDbCommand _command;
        private IDbTransaction _transaction;

        private string _connectionString = "";
         

        #region "Properties"

        #region protected

        public ConnectionState ConnectionState
        {
            get
            {
                return Connection.State;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                //if (_connection == null)
                //{
                //    CreateDatabaseObjects();
                //}
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public IDbDataAdapter DataAdapter
        {
            get
            {
                //if (_dataAdapter == null)
                //{
                //    CreateDatabaseObjects();
                //}
                return _dataAdapter;
            }
            set
            {
                _dataAdapter = value;
            }
        }

        public IDbCommand Command
        {
            get
            {
                //if (_command == null)
                //{
                //    CreateDatabaseObjects();
                //}
                return _command;
            }
            set
            {
                _command = value;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
            //set
            //{
            //    _transaction = value;
            //}
        }

        #endregion

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                this._connectionString = value;
                if (this.Connection != null)
                {
                    this.Connection.ConnectionString = this._connectionString;
                }
            }
        }

        public IDataParameterCollection Parameters
        {
            get
            {
                return this.Command.Parameters;
            }
        }
 
        #endregion

        internal protected Database()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._transaction != null)
                {
                    this._transaction.Dispose();
                }

                if (this.Command != null)
                {
                    this.Command.Dispose();
                }

                if (this.Connection != null)
                {
                    this.Connection.Dispose();
                }
            }
        }

        #region Connection


        private void RaiseExceptionEvent(ConnectionException newException,
          ConnectionOperation operation)
        {
            var args = new ConnectionEventArgs
            {
                Exception = newException,
                ThrowException = true,
                Operation = operation
            };

            ExceptionManager.RaiseExceptionEvent(this.Connection, args);
            if (args.ThrowException)
            {
                throw newException;
            }
        }

        /// <summary>
        /// Connects to a database.
        /// When exception occurs,it will clear all connections in pool.
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (string.IsNullOrEmpty(this._connectionString))
            {
                throw new ConnectionException(MsgResource.ConnectionStringMissing);
            }

            try
            {
                this.Connection.ConnectionString = this._connectionString;
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
                this.Connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                //this.ClearPool();
                this.Dispose();
                this.ClearAllPools();

                var newException = new ConnectionException(MsgResource.OpenDatabaseFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.Open);

                return false;
            }
        }


        /// <summary>
        /// Closes the connection to the database.
        /// When exception occurs,it will clear all connections in pool.
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                //this.ClearPool();
                this.Dispose();
                this.ClearAllPools();

                var newException = new ConnectionException(MsgResource.CloseDatabaseFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.Close);

                return false;
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public bool ChangeDatabase(string databaseName)
        {
            try
            {
                this.Connection.ChangeDatabase(databaseName);
                return true;
            }
            catch (Exception ex)
            {
                this.ClearPool();
                this.Dispose();
                //this.ClearAllPools();

                var newException = new ConnectionException(MsgResource.ChangeDatabaseFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.ChangeDatabase);

                return false;
            }
        }

        /// <summary>
        /// Clears all connections in pool.
        /// </summary>
        public abstract void ClearAllPools();

        /// <summary>
        /// Clears current connection in pool.
        /// </summary>
        public abstract void ClearPool();

        #endregion

        #region Transaction

        /// <summary>
        /// Begins a database transaction with 60s active time.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            try
            {
                this._transaction = this.Connection.BeginTransaction();
                return this._transaction;
            }
            catch (Exception ex)
            {
                this._transaction = null;

                var newException = new ConnectionException(MsgResource.BeginTransactionFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.BeginTransaction);

                return null;
            }
        }

        /// <summary>
        /// Begins a database transaction with specified IsolationLevel value.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                this._transaction = this.Connection.BeginTransaction(isolationLevel);
                return this._transaction;
            }
            catch (Exception ex)
            {
                this._transaction = null;

                var newException = new ConnectionException(MsgResource.BeginTransactionFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.BeginTransaction);

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            try
            {
                if (this._transaction != null)
                {
                    this._transaction.Commit();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var newException = new ConnectionException(MsgResource.CommitTransactionFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.Commit);

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            try
            {
                if (this._transaction != null)
                {
                    this._transaction.Rollback();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var newException = new ConnectionException(MsgResource.RollbackTransactionFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, ConnectionOperation.Rollback);

                return false;
            }
        }

        private void OnException()
        {
            this.Rollback();
            this.Close();
            this.Dispose();
        }

        #endregion


        public ITableCommandExecutor CreateTableCommandExecutor()
        {
            return new CommandExecutor(this);
        }

        public IViewCommandExecutor CreateViewCommandExecutor()
        {
            return new CommandExecutor(this);
        }

        public override string ToString()
        {
            return this._connectionString;
        }


        #region CommandParameter

        private void ResetCommandType(bool isStoredProcedure)
        {
            if (isStoredProcedure)
            {
                this.Command.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                this.Command.CommandType = CommandType.Text;
            }
        }

        private void ResetCommandParameters()
        {
            this.Command.Parameters.Clear();
        }
        
        private void ResetCommandParameters(IDataParameter parameter)
        {
            this.Command.Parameters.Clear();
            this.Command.Parameters.Add(parameter);
        }

        private void ResetCommandParameters(IList<IDataParameter> parameters)
        {
            this.Command.Parameters.Clear();
            foreach (IDataParameter parameter in parameters)
            {
                this.Command.Parameters.Add(parameter);
            }
        }
 
        #endregion


        #region ExecuteReader

        private IDataReader ExecuteReader(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new CommandException(MsgResource.CommandTextMissing);
            }
            try
            {
                this.Command.Transaction = this._transaction;
                this.Command.CommandText = commandText;
                return Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                var newException = new CommandException(MsgResource.ExecuteReaderFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, CommandOperation.ExecuteReader);

                return null;
            }

        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters();
            return ExecuteReader(commandText);

        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure, IDataParameter parameter)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameter);
            return ExecuteReader(commandText);

        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteReader(commandText);

        }
 
        #endregion


        #region ExecuteNonQuery

      
        private int ExecuteNonQuery(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new CommandException(MsgResource.CommandTextMissing);
            }
            int rowCount = 0;
            try
            {
                this.Command.Transaction = this._transaction;
                this.Command.CommandText = commandText;
                rowCount = Command.ExecuteNonQuery();
                return rowCount;
            }
            catch (Exception ex)
            {
                var newException = new CommandException(MsgResource.ExecuteNonQueryFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, CommandOperation.ExecuteNonQuery);

                return 0;
            }

        }
 
        public int ExecuteNonQuery(string commandText, bool isProcedure)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters();
            return ExecuteNonQuery(commandText);

        }
 
        public int ExecuteNonQuery(string commandText, bool isProcedure, IDataParameter parameter)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameter);
            return ExecuteNonQuery(commandText);

        }

       
        public int ExecuteNonQuery(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteNonQuery(commandText);

        }
 

        #endregion


        #region ExecuteScalar

       
        private object ExecuteScalar(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new CommandException(MsgResource.CommandTextMissing);
            }
            try
            {
                Command.Transaction = this._transaction;
                Command.CommandText = commandText;
                return Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                var newException = new CommandException(MsgResource.ExecuteScalarFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, CommandOperation.ExecuteScalar);

                return null;
            }
        }

        /// <summary>
        /// Executes the query,and returns the first column of the first row.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, bool isProcedure)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters();
            return ExecuteScalar(commandText);
        }

        /// <summary>
        ///  Executes the query,and returns the first column of the first row.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, bool isProcedure, IDataParameter parameter)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameter);
            return ExecuteNonQuery(commandText);
        }

        /// <summary>
        ///  Executes the query,and returns the first column of the first row.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteScalar(commandText);
        }

        #endregion


        #region ExecuteDataSet


        private DataSet ExecuteDataSet(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new CommandException(MsgResource.CommandTextMissing);
            }
            try
            {
                this.Command.CommandText = commandText;
                DataSet dataSet = new DataSet("NewDataSet");
                dataSet.Locale = CultureInfo.InvariantCulture;
                this.DataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                var newException = new CommandException(MsgResource.GetDataSetFailed + ex.Message, ex);
                RaiseExceptionEvent(newException, CommandOperation.ExecuteDataSet);

                return null;
            }
        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters();
            return ExecuteDataSet(commandText);
        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure, IDataParameter parameter)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameter);
            return ExecuteDataSet(commandText);

        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteDataSet(commandText);
        }

        #endregion


        #region ExecuteDataTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, bool isProcedure)
        {
            DataSet ds = ExecuteDataSet(commandText, isProcedure);
            return GetDataTable(ds);
        }

        private static DataTable GetDataTable(DataSet ds)
        {
            if (ds == null)
            {
                return null;
            }
            return ds.Tables.Count > 0 ? ds.Tables[0].Copy() : new DataTable("NewDataTable");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, bool isProcedure, IDataParameter parameter)
        {
            DataSet ds = ExecuteDataSet(commandText, isProcedure, parameter);
            return GetDataTable(ds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="isProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, bool isProcedure, IList<IDataParameter> parameters)
        {
            DataSet ds = ExecuteDataSet(commandText, isProcedure, parameters);
            return GetDataTable(ds);
        }


        #endregion



        private void RaiseExceptionEvent(CommandException newException,
          CommandOperation operation)
        {
            var args = new CommandEventArgs
            {
                Exception = newException,
                ThrowException = true,
                Operation = operation
            };

            ExceptionManager.RaiseExceptionEvent(this.Command, args);
            if (args.ThrowException)
            {
                throw newException;
            }
        }
    }
}
