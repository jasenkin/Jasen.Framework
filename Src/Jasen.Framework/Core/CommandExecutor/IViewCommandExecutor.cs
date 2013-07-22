using System;
using System.Data;
using System.Collections.Generic;

namespace Jasen.Framework
{
    public interface IViewCommandExecutor
    {
        IDatabase Database
        {
            get;
        }

        DataSet ExecuteDataSet(string commandText, bool isProcedure);

        DataSet ExecuteDataSet(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        DataSet ExecuteDataSet(string commandText, bool isProcedure, IDataParameter parameter);

        DataTable ExecuteDataTable(string commandText, bool isProcedure, IDataParameter parameter);

        DataTable ExecuteDataTable(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        DataTable ExecuteDataTable(string commandText, bool isProcedure);

        IDataReader ExecuteReader(string commandText, bool isProcedure);

        IDataReader ExecuteReader(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        IDataReader ExecuteReader(string commandText, bool isProcedure, IDataParameter parameter);

        object ExecuteScalar(string commandText, bool isProcedure);

        object ExecuteScalar(string commandText, bool isProcedure, IDataParameter parameter);


        object ExecuteScalar(string commandText, bool isProcedure, IList<IDataParameter> parameters);

        string ToString();
    }
}
