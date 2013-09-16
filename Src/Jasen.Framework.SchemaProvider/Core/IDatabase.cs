using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Jasen.Framework
{
    [ComVisible(false)]
    public interface IDatabase
    {
        string ConnectionString { get; set; }
        ConnectionState ConnectionState { get; }
        IDbConnection Connection{get;  }
        IDbDataAdapter DataAdapter {  get; }
        IDbCommand Command { get; }  
        IDbTransaction Transaction{  get;  }
        IDataParameterCollection Parameters {   get;   }

        bool Close();

        bool Open();

        void ClearAllPools();

        void ClearPool();

        bool ChangeDatabase(string databaseName);

        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);

        bool Commit();

        bool Rollback();

        IDataReader ExecuteReader(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null);

        int ExecuteNonQuery(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null);

        object ExecuteScalar(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null);

        DataSet ExecuteDataSet(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null);

        DataTable ExecuteDataTable(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null);
    }
}