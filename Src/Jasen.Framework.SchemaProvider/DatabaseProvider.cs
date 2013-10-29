using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Jasen.Framework.MetaData;
using Jasen.Framework.SchemaProvider;
using Jasen.Framework.CodeGenerator;

namespace Jasen.Framework
{
    public abstract class DatabaseProvider :IDatabaseProvider
    {
        public DatabaseProvider()
        { 
        }

        private CodeProvider _codeProvider;

        public virtual CodeProvider CodeProvider
        {
            get
            {
                if(this._codeProvider==null)
                {
                    this._codeProvider = new CodeProvider();
                }

                return this._codeProvider;
            }
        }

        private string _connectionString;

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this._connectionString = value;
                this.Database.ConnectionString = value;
            }
        }

        public bool IsInited { get; protected set; }

        public abstract IDatabase Database { get; }

        public IList<string> TableNames { get; protected set; }

        public IList<string> ViewNames { get; protected set; }

        public IList<string> ProcedureNames { get; protected set; }

        public IList<PrimaryKey> PrimaryKeys { get; protected set; }

        public IList<ForeignKey> ForeignKeys { get; protected set; }

        public IList<IdentityKey> IdentityKeys { get; protected set; }

        public IList<ProcedureParameter> ProcedureParameters { get; protected set; }

        public virtual void Init()
        { 
            this.TableNames = this.GetTableNames()?? new List<string>();
            this.ViewNames = this.GetViewNames() ?? new List<string>();
            this.ProcedureParameters = this.GetProcedureParameters() ?? new List<ProcedureParameter>();

            foreach (var param in this.ProcedureParameters)
            {
                param.ParameterType = GetDataType(param.ParameterType);
            }

            this.ProcedureNames = this.GetProcedureNames() ?? new List<string>();
            this.PrimaryKeys = this.GetAllPrimaryKeys() ?? new List<PrimaryKey>();
            this.ForeignKeys = this.GetAllForeignKeys() ?? new List<ForeignKey>();
            this.IdentityKeys = this.GetAllIdentityKeys() ?? new List<IdentityKey>();
            this.IsInited = true;
        }

        public virtual IEnumerable<TableColumn> GetTableOrViewInfo(string tableNameOrViewName)
        {
            if(string.IsNullOrWhiteSpace(tableNameOrViewName))
            {
                return new List<TableColumn>();
            }

            if (!this.IsInited)
            {
                this.Init();
            }

            this.Database.Connection.Open();
            DataTable tempTable = this.Database.ExecuteDataTable(CreateSelectCommandText(tableNameOrViewName), false);
            this.Database.Connection.Close();
            tempTable.TableName = tableNameOrViewName;

            if(tempTable.Columns.Count==0)
            {
                return new List<TableColumn>();
            }

            IList<TableColumn> tableColumns = new List<TableColumn>();
            TableColumn tableColumn;
            ForeignKey foreignKey;

            foreach (DataColumn column in tempTable.Columns)
            {
                tableColumn = new TableColumn();
                tableColumn.TableName = tableNameOrViewName;
                tableColumn.ColumnName = column.ColumnName;
                tableColumn.DataType = GetDataType(column);

                tableColumn.IsPrimaryKey = this.PrimaryKeys.Any(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, column.ColumnName));
                tableColumn.IsIdentity = this.IdentityKeys.Any(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, column.ColumnName));

                foreignKey = this.ForeignKeys.FirstOrDefault(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, column.ColumnName));

                if (foreignKey != null)
                {
                    tableColumn.IsForeignKey = true;
                    tableColumn.ReferenceTableName = foreignKey.ReferenceTableName;
                    tableColumn.ReferenceKey = foreignKey.ReferenceKey;
                }
                
                tableColumns.Add(tableColumn);
            }

            return tableColumns;
        }

        protected virtual string CreateSelectCommandText(string tableNameOrViewName)
        {
            return "select * from " + tableNameOrViewName.Trim() + " where null = null";
        }

        public virtual IEnumerable<ProcedureParameter> GetProcedureParameters(string procedureName)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return new List<ProcedureParameter>();
            }

            if (!this.IsInited)
            {
                this.Init();
            }

            var procedures = this.ProcedureParameters.Where(item=>string.Equals(item.ProcedureName,procedureName.Trim()));

            return procedures;
        }

        public virtual string GetDataType(DataColumn column)
        {
            string dataType = "";
            switch (column.DataType.Name)
            {
                case "Int32": 
                    dataType = "int";
                    break;
                case "String": 
                    dataType = "string";
                    break;
                case "Boolean": 
                    dataType = "bool";
                    break;
                case "DateTime": 
                    dataType = "DateTime?";
                    break;
                case "Byte": 
                    dataType = "byte";
                    break;
                case "Byte[]": 
                    dataType = "byte[]";
                    break;
                default: 
                    dataType = column.DataType.Name;
                    break;
            }

            return dataType;
        }

        public virtual string GetDataType(string databaseType)
        {
            return SqlServerConverter.ToCSharpType(databaseType);
        }

        public abstract IList<string> GetTableNames();

        public abstract IList<string> GetViewNames();

        public abstract IList<string> GetProcedureNames(); 

        public abstract IList<PrimaryKey> GetAllPrimaryKeys(); 

        public abstract IList<ForeignKey> GetAllForeignKeys();

        public abstract IList<IdentityKey> GetAllIdentityKeys();

        public virtual List<ProcedureParameter> GetProcedureParameters()
        {
            return new List<ProcedureParameter>();
        }
         
        public string Build(string name, OperationType operationType, string nameSpace, 
            bool addAttribute, string templateFilePath)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if(!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException(templateFilePath);
            }

            if (operationType == OperationType.Procedure)
            {
                IEnumerable<ProcedureParameter> parameters = this.GetProcedureParameters(name.Trim());
                return this.CodeProvider.BuildProcedure(name, nameSpace, templateFilePath, parameters);
                
            }
            else if (operationType == OperationType.Table)
            {
                IEnumerable<TableColumn> columns = this.GetTableOrViewInfo(name.Trim());
                return this.CodeProvider.BuildTable(name, nameSpace, addAttribute, templateFilePath, columns);
            }
            else
            {
                IEnumerable<TableColumn> columns = this.GetTableOrViewInfo(name.Trim());
                return this.CodeProvider.BuildView(name, nameSpace, addAttribute, templateFilePath, columns);
            }
        }
    }
}
