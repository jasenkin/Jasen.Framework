using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jasen.Framework.MetaData;
using Jasen.Framework.SchemaProvider;
using Jasen.Framework.CodeGenerator;

namespace Jasen.Framework.OracleSchemaProvider
{  
     public class OracleProvider: DatabaseProvider
    {
        private CodeProvider _codeProvider;

        public override CodeProvider CodeProvider
        {
            get
            {
                if (this._codeProvider == null)
                {
                    this._codeProvider = new OracleCodeProvider();
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
                    this._database = new OracleDatabase();
                }

                return this._database;
            }
        }

        public OracleProvider()
        {
            
        }

        public override IList<string> GetTableNames()
        {
            string sql = @"SELECT decode(a.owner,null,u.table_name,a.owner || '.' || u.table_name) TableName FROM USER_TABLES u " +
                " LEFT JOIN ALL_TABLES a ON u.table_name=a.table_name AND u.tablespace_name=a.tablespace_name " +
                " ORDER BY a.owner,u.table_name ";

            this.Database.Connection.Open();
            DataTable tableNames = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();

           IList<string> names =new List<string>();

            foreach (DataRow row in tableNames.Rows)
            {
                names.Add(row["TableName"].ToString());
            }

            return names; 
        }

        public override IList<string> GetViewNames()
        {
            string sql = @"SELECT decode(a.owner,null,u.view_name,a.owner || '.' || u.view_name) TableName FROM USER_VIEWS u " +
                " LEFT JOIN ALL_VIEWS a ON u.view_name=a.view_name " +
                " ORDER BY a.owner,u.view_name "; 
  
            this.Database.Connection.Open();
            DataTable tableNames = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();

           IList<string> names =new List<string>();

            foreach (DataRow row in tableNames.Rows)
            {
                names.Add(row["TableName"].ToString());
            }

            return names; 
        }

        public override IList<string> GetProcedureNames()
        {
            return this.GetProcedureParameters().Select(p=>p.ProcedureName).Distinct().ToList();
        }
 
        public override IList<PrimaryKey> GetAllPrimaryKeys()
        {
            string sql = @"select col.table_name TableName,col.column_name PrimaryKey from user_constraints con,  user_cons_columns col " +
                "where con.constraint_name = col.constraint_name and con.constraint_type='P'";

            this.Database.Connection.Open();
            DataTable primaryKeyTable = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();
 
                if (primaryKeyTable == null || primaryKeyTable.Columns.Count <= 0)
                {
                    return new List<PrimaryKey>();
                }

            IList<PrimaryKey> primartKeys =new List<PrimaryKey>();
            PrimaryKey primaryKey;

                foreach (DataRow row in primaryKeyTable.Rows)
                {
                    primaryKey =new PrimaryKey();
                    primaryKey.TableName =row["TableName"].AsString();
                    primaryKey.Key =row["PrimaryKey"].AsString();
                   
                    primartKeys.Add(primaryKey);
                }

            return primartKeys;
        }

        public override IList<ForeignKey> GetAllForeignKeys()
        {
            string sql = @"select con.table_name TableName,col.column_name ForeignKey,r.table_name ReferenceTableName,r.column_name ReferenceKey from " +
                "user_constraints con,user_cons_columns col, (select t2.table_name,t2.column_name,t1.r_constraint_name " +
                "from user_constraints t1,user_cons_columns t2 where t1.r_constraint_name=t2.constraint_name) r " +
                "where con.constraint_name=col.constraint_name and con.r_constraint_name=r.r_constraint_name";

            this.Database.Connection.Open();
            DataTable foreignKeyTable = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();

            List<ForeignKey> keys = new List<ForeignKey>();
             ForeignKey foreignKey;

            foreach (DataRow row in foreignKeyTable.Rows)
            {
                foreignKey = new ForeignKey();
                foreignKey.TableName = row["TableName"].ToString();
                foreignKey.Key = row["ForeignKey"].ToString();
                foreignKey.ReferenceTableName = row["ReferenceTableName"].ToString();
                foreignKey.ReferenceKey = row["ReferenceKey"].ToString();
                keys.Add(foreignKey);
            }

            return keys; 
        }

        public override IList<IdentityKey> GetAllIdentityKeys()
        {
            return new List<IdentityKey>();
        }

        public override List<ProcedureParameter> GetProcedureParameters()
        {
            string sql = @"select decode(t.package_name,null,t.object_name,t.package_name || '.' || t.object_name) || t.overload ProcedureName, " +
                " t.argument_name ParameterName,t.data_length Length,t.in_out InOut,t.data_type ParameterType " +
                    "from user_arguments t " +
                    " order by t.package_name,t.object_name ";
            this.Database.Connection.Open();
            DataTable procedureParameterTable = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();

            List<ProcedureParameter> procedureParameters = new List<ProcedureParameter>();

            foreach (DataRow row in procedureParameterTable.Rows)
            {
                ProcedureParameter parameter = new ProcedureParameter();
                parameter.ProcedureName = row["ProcedureName"].ToString();
                parameter.ParameterName = row["ParameterName"].ToString();
                parameter.ParameterType = row["ParameterType"].ToString();
                parameter.Length = row["Length"].ToString();
                parameter.IsOutput = row["InOut"].ToString();
                procedureParameters.Add(parameter);
            }
            return procedureParameters;
        }

        public override string GetDataType(string databaseType)
        {
            return OracleConverter.ToCSharpType(databaseType);
        }

    }
}
