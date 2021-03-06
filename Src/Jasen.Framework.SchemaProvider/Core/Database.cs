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
         

        public bool ExistTransaction
        {
            get;
            private set;
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

        public bool Open()
        {
            if (string.IsNullOrEmpty(this._connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            try
            {
                if (this.Connection.State == ConnectionState.Closed)
                {
                    this.Connection.ConnectionString = this._connectionString;
                    this.Connection.Open();
                }

                return true;
            }
            catch (Exception ex)
            { 
                this.Dispose();
                this.ClearAllPools();  
                ExceptionManager.RaiseExceptionEvent(ex);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (this._transaction == null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            { 
                this.Dispose();
                this.ClearAllPools();

                ExceptionManager.RaiseExceptionEvent(ex);

                return false;
            }
        }

        public bool ChangeDatabase(string databaseName)
        {
            try
            {
                this.Connection.ChangeDatabase(databaseName);
                return true;
            }
            catch (Exception ex)
            {
                this.Dispose();
                this.ClearAllPools();

                ExceptionManager.RaiseExceptionEvent(ex);

                return false;
            }
        }
         
        public abstract void ClearAllPools();
         
        public abstract void ClearPool();

        #endregion
         
        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            try
            {
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    throw new ArgumentNullException("ConnectionString is null");
                }
                 
                if (this.ConnectionState == ConnectionState.Closed)
                {
                    if (this.Open())
                    {
                        BeginAndSetTransaction(isolationLevel);
                    }
                    else
                    {
                        this._transaction = null;
                    }
                }
                else if (this.ConnectionState == ConnectionState.Open && this._transaction == null)
                {
                    BeginAndSetTransaction(isolationLevel);
                }

                return this._transaction; 
            }
            catch (Exception ex)
            {
                this._transaction = null;

                ExceptionManager.RaiseExceptionEvent(ex);

                return null;
            }
        }

        private void BeginAndSetTransaction(IsolationLevel? isolationLevel)
        {
            if (isolationLevel.HasValue)
            {
                this._transaction = this.Connection.BeginTransaction(isolationLevel.Value);
            }
            else
            {
                this._transaction = this.Connection.BeginTransaction();
            }
        }

        public bool Commit()
        {
            try
            {
                if (this.ConnectionState == ConnectionState.Open)
                {
                    if (this._transaction != null)
                    {
                        this._transaction.Commit();
                        return true;
                    } 
                } 

                return false;
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);

                return false;
            }
            finally
            {
                this._transaction = null;
                this.Close();
            }
        }
         
        public bool Rollback()
        {
            try
            {
                if (this.ConnectionState == ConnectionState.Open)
                {
                    if (this._transaction != null)
                    {
                        this._transaction.Rollback();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);
                return false;
            }
            finally
            {
                this._transaction = null;
                this.Close();
            }
        } 
         
        public override string ToString()
        {
            return this._connectionString;
        }
         

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

        private void ResetCommandParameters(IList<IDataParameter> parameters)
        {

            this.Command.Parameters.Clear();

            if (parameters == null)
            {
                return;
            }

            foreach (IDataParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    this.Command.Parameters.Add(parameter);
                }
            }
        } 

        private IDataReader ExecuteReader(string commandText)
        {
            Open();

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }
            try
            {
                this.Command.Transaction = this._transaction;
                this.Command.CommandText = commandText;
                return Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);

                return null;
            }
        } 

        public IDataReader ExecuteReader(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            Open();

            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteReader(commandText);

        }
  
        private int ExecuteNonQuery(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            int rowCount = 0;
            try
            {
                Open();

                this.Command.Transaction = this._transaction;
                this.Command.CommandText = commandText;
                rowCount = Command.ExecuteNonQuery();

                Close();
                return rowCount;
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);
                return 0;
            }

        } 

        public int ExecuteNonQuery(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteNonQuery(commandText);

        } 

        private object ExecuteScalar(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }
            try
            {
                Open();

                Command.Transaction = this._transaction;
                Command.CommandText = commandText;
                object result = Command.ExecuteScalar();

                Close();

                return result;
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);
                return null;
            }
        }

        public object ExecuteScalar(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteScalar(commandText);
        }

        private DataSet ExecuteDataSet(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            try
            {
                Open();

                this.Command.CommandText = commandText;
                DataSet dataSet = new DataSet("NewDataSet");
                dataSet.Locale = CultureInfo.InvariantCulture;
                this.DataAdapter.Fill(dataSet);

                Close();

                return dataSet;
            }
            catch (Exception ex)
            {
                ExceptionManager.RaiseExceptionEvent(ex);

                return null;
            }
        } 

        public DataSet ExecuteDataSet(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            ResetCommandType(isProcedure);
            ResetCommandParameters(parameters);
            return ExecuteDataSet(commandText);
        }

        public DataTable ExecuteDataTable(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            DataSet ds = ExecuteDataSet(commandText, isProcedure, parameters);

            if (ds == null)
            {
                return null;
            }

            return ds.Tables.Count > 0 ? ds.Tables[0].Copy() : new DataTable("NewDataTable");
        }
    }
}
