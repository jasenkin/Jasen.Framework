using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Jasen.Framework.Configuration;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;

namespace Jasen.Framework
{
    public class StoreProdureExecutor<T> : IStoreProcedure where T : StoreProdureExecutor<T>, new()
    {
        private ITableCommandExecutor _commandExecutor;

        protected DatabaseConfig Config
        {
            get
            {
                return ConfigManager.Current.FindConfig(typeof(T));
            }
        }

        protected ITableCommandExecutor CommandExecutor
        {
            get
            {
                if(this._commandExecutor!=null)
                {
                    DatabaseConfig config = this.Config;
                    IDatabase database = DatabaseFactory.CreateInstance(config.DatabaseType);
                    database.ConnectionString = config.ConnectionString;
                    database.Command.CommandTimeout = config.CommandTimeout;
                    this._commandExecutor = database.CreateTableCommandExecutor();     
                }

                return this._commandExecutor;
            }
        }

        public StoreProdureExecutor()
        {

        }
 
        public bool BeginTransaction()
        {
            if (this.CommandExecutor == null)
            {
                return false;
            }

            ProcedureAttribute procedureAttribute = AttributeUtility.GetProcedureAttribute(typeof(T));

            if (procedureAttribute == null)
            {
                return false;
            }

            return this.CommandExecutor.BeginTransaction() != null;
        }

        public bool Commit()
        {
            if (this.CommandExecutor == null)
            {
                return false;
            }

            return this.CommandExecutor.Commit();
        }

        public bool Rollback()
        {
            if (this.CommandExecutor == null)
            {
                return false;
            }

            return this.CommandExecutor.Rollback();
        }

        public int ExecuteNonQuery()
        {
            if (this.CommandExecutor == null)
            {
                return 0;
            }

            string procedureName = GetProcedureName();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, this, this._commandExecutor.Database.DatabaseType);
            int count = this.CommandExecutor.ExecuteNonQuery(procedureName, true, dataParameters);
            UpdateOutputProperties(this, dictionary, dataParameters);

            return count;
        }

        public object ExecuteScalar()
        {
            if (this.CommandExecutor == null)
            {
                return null;
            }

            string procedureName = GetProcedureName();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, this, this._commandExecutor.Database.DatabaseType);
            object obj = this.CommandExecutor.ExecuteScalar(procedureName, true, dataParameters);
            UpdateOutputProperties(this, dictionary, dataParameters);

            return obj;
        }

        public IDataReader ExecuteReader()
        {
            if (this.CommandExecutor == null)
            {
                return null;
            }

            string procedureName = GetProcedureName();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, this, this._commandExecutor.Database.DatabaseType);
            IDataReader reader = this.CommandExecutor.ExecuteReader(procedureName, true, dataParameters);
            UpdateOutputProperties(this, dictionary, dataParameters);

            return reader;
        }

        public void CloseReader(IDataReader reader)
        {
            if (!reader.IsClosed)
            {
                reader.Close();
            }

            if (this.CommandExecutor != null && this.CommandExecutor.Database.ConnectionState != ConnectionState.Closed)
            {
                this.CommandExecutor.Database.Close();
            }
        }

        public DataTable ExecuteDataTable()
        {
            if (this.CommandExecutor == null)
            {
                return new DataTable();
            }

            string procedureName = GetProcedureName();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, this, this._commandExecutor.Database.DatabaseType);
            DataTable dt = this.CommandExecutor.ExecuteDataTable(procedureName, true, dataParameters);
            UpdateOutputProperties(this, dictionary, dataParameters);

            return dt;
        }

        public DataTable ExecuteDataTable<TTarget>() where TTarget : new()
        {
            DataTable dt = ExecuteDataTable();
            EntityMapper<TTarget>.AdaptToEntity(dt);
            return dt;
        }

        public IList<TTarget> ExecuteEntity<TTarget>() where TTarget : new()
        {
            DataTable dt = ExecuteDataTable();
            EntityMapper<TTarget>.AdaptToEntity(dt);
            return EntityMapper<TTarget>.ToEntities(dt);
        }

        public DataSet ExecuteDataSet()
        {
            if (this._commandExecutor == null)
            {
                return new DataSet();
            }

            string procedureName = GetProcedureName();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, this, this._commandExecutor.Database.DatabaseType);
            DataSet ds = this.CommandExecutor.ExecuteDataSet(procedureName, true, dataParameters);
            UpdateOutputProperties(this, dictionary, dataParameters);
            return ds;
        }

        private static string GetProcedureName()
        {
            var procedureAttribute = AttributeUtility.GetProcedureAttribute(typeof(T));

            if (procedureAttribute == null)
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            return string.IsNullOrEmpty(procedureAttribute.Name) ? typeof(T).Name : procedureAttribute.Name;
        }

        private static void UpdateOutputProperties(object entity, IDictionary<ParameterAttribute, PropertyInfo> parameterAttributes,
            IEnumerable<IDataParameter> dataParameters)
        {
            foreach (IDataParameter parameter in dataParameters)
            {
                if (parameter.Direction == ParameterDirection.Input)
                {
                    continue;
                }

                ParameterAttribute parameterAttr = parameterAttributes.Keys.FirstOrDefault(p => string.Equals(p.Name, parameter.ParameterName));

                if (parameterAttr == null)
                {
                    continue;
                }

                if (parameterAttributes[parameterAttr].PropertyType == typeof(OracleCursor))
                {
                    continue;
                }

                parameterAttributes[parameterAttr].SetValue(entity, parameter.Value, null);
            }
        }

        private static IList<IDataParameter> ToDataParameters(IDictionary<ParameterAttribute, PropertyInfo> dictionary,
             object entity, DatabaseType databaseType)
        {
            if (dictionary == null || dictionary.Keys.Count == 0)
            {
                return new List<IDataParameter>();
            }

            var dataParameters = new List<IDataParameter>();
            ISqlBuilder parameterBuilder = SqlBuilderFactory.CreateInstance(databaseType);
            IDataParameter dataParameter;
            string name;
            object value;

            foreach (var parameterKey in dictionary.Keys)
            {
                name = string.IsNullOrEmpty(parameterKey.Name) ? dictionary[parameterKey].Name : parameterKey.Name;
                value = dictionary[parameterKey].GetValue(entity, null);

                dataParameter = parameterBuilder.CreateProcedureParameter(parameterKey, name, value);

                if (dataParameter != null)
                {
                    dataParameters.Add(dataParameter);
                }
            }

            return dataParameters;
        }
    }
}
