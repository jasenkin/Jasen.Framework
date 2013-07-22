using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jasen.Framework.MetaData;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.MySqlSchemaProvider
{
    public class MySqlProvider : DatabaseProvider
    {
        private CodeProvider _codeProvider;

        public override CodeProvider CodeProvider
        {
            get
            {
                if (this._codeProvider == null)
                {
                    this._codeProvider = new MySqlCodeProvider();
                }

                return this._codeProvider;
            }
        }

        private IDatabase _database;
        public override IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = new MySqlDatabase();
                }

                return this._database;
            }
        }

        public string DatabaseName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(this.Database.ConnectionString))
                {
                    throw new ArgumentNullException("ConnectionString is invalid.");
                }

                return this.Database.Connection.Database;
            }
        } 

        public override IList<string> GetTableNames()
        {
            string tableSql = @"SELECT  TABLE_NAME  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='{0}'";
            DataTable table = GetTable(tableSql);

            return ConvertToNames(table, "TABLE_NAME");
        }

        public override IList<string> GetViewNames()
        {
            string viewSql = @"SELECT  TABLE_NAME  FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA='{0}'";
            DataTable table = GetTable(viewSql);

            return ConvertToNames(table, "TABLE_NAME");
        }

        public override IList<string> GetProcedureNames()
        {
            string procedureSql = @"SELECT Name FROM mysql.proc  WHERE db = '{0}'  and  type =  'PROCEDURE'";
            DataTable procedureTable = GetTable(procedureSql);

            return ConvertToNames(procedureTable, "Name");
        } 

        public override IList<PrimaryKey> GetAllPrimaryKeys()
        {
            string primarySql = @"SELECT  TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                  WHERE TABLE_SCHEMA='{0}' and CONSTRAINT_NAME='PRIMARY'";

            DataTable primaryTable = GetTable(primarySql);

            if (primaryTable.Rows.Count == 0)
            {
                return new List<PrimaryKey>();
            }

            var primaryKeys = new List<PrimaryKey>();
            PrimaryKey primaryKey;

            foreach (DataRow row in primaryTable.Rows)
            {
                primaryKey = new PrimaryKey();
                primaryKey.TableName = row["TABLE_NAME"].AsString();;
                primaryKey.Key = row["COLUMN_NAME"].AsString(); ;

                primaryKeys.Add(primaryKey);
            }

            return primaryKeys;
        }

        private DataTable GetTable(string primarySql)
        {
            this.Database.Connection.Open();
            DataTable primaryTable = this.Database.ExecuteDataTable(string.Format(primarySql, this.DatabaseName), false);
            this.Database.Connection.Close();
            return primaryTable;
        }

        public override IList<ForeignKey> GetAllForeignKeys()
        {
            string foreignSql =  @"SELECT TABLE_NAME,COLUMN_NAME,
                  REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                  where  CONSTRAINT_NAME<>'PRIMARY' and TABLE_SCHEMA='{0}'";
       
            DataTable foreignKeyTable = GetTable(foreignSql);

            if (foreignKeyTable == null || foreignKeyTable.Rows.Count == 0)
            {
                return new List<ForeignKey>();
            }

            IList<ForeignKey> foreignKeys = new List<ForeignKey>();
            ForeignKey foreignKey = null;

            foreach (DataRow row in foreignKeyTable.Rows)
            {
                foreignKey = new ForeignKey();
                foreignKey.TableName = row["TABLE_NAME"].AsString();
                foreignKey.Key = row["COLUMN_NAME"].AsString();
                foreignKey.ReferenceTableName = row["REFRENCED_TABLE_NAME"].AsString();
                foreignKey.ReferenceKey = row["REFRENCED_COLUMN_NAME"].AsString();
                foreignKeys.Add(foreignKey);
            }

            return foreignKeys;
        }

        public override IList<IdentityKey> GetAllIdentityKeys()
        {
            string identitySql = @"SELECT  TABLE_NAME,COLUMN_NAME  FROM INFORMATION_SCHEMA.Columns
                  WHERE TABLE_SCHEMA='{0}' AND EXTRA ='auto_increment' AND COLUMN_KEY = 'PRI'";
           
            DataTable identityTable = GetTable(identitySql);

            if (identityTable == null || identityTable.Rows.Count == 0)
            {
                return new List<IdentityKey>();
            }

            IList<IdentityKey> identityKeys = new List<IdentityKey>();
            IdentityKey identityKey = null;

            foreach (DataRow row in identityTable.Rows)
            {
                identityKey = new IdentityKey();
                identityKey.TableName = row["TABLE_NAME"].AsString(); ;
                identityKey.Key = row["COLUMN_NAME"].AsString(); ;
                identityKeys.Add(identityKey);      
            }

            return identityKeys;
        }

        public override List<ProcedureParameter> GetProcedureParameters()
       {
            string parameterSql=@"SELECT  SPECIFIC_NAME AS PROCEDURE_NAME,DATA_TYPE,
            PARAMETER_MODE,PARAMETER_NAME FROM INFORMATION_SCHEMA.parameters  WHERE SPECIFIC_SCHEMA='{0}' ";

            DataTable procedureParameterTable = GetTable(parameterSql);

            var procedureParameters = new List<ProcedureParameter>();

            foreach (DataRow row in procedureParameterTable.Rows)
            {
                var parameter = new ProcedureParameter();
                parameter.ProcedureName = row["PROCEDURE_NAME"].AsString();
                parameter.ParameterName = row["PARAMETER_NAME"].AsString();
                parameter.ParameterType = row["DATA_TYPE"].AsString();
                parameter.IsOutput = string.Equals(row["PARAMETER_MODE"].AsString(), "OUT") ? "1" : "0";
            
                procedureParameters.Add(parameter);
            }

            return procedureParameters;
        }

        private static IList<string> ConvertToNames(DataTable table,string nameMark)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return new List<string>();
            }

            var names = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                names.Add(row[nameMark.Trim()].AsString());
            }

            return names;
        }

    }
}
