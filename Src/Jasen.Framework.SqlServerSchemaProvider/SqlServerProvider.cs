using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jasen.Framework.MetaData;

namespace Jasen.Framework.SqlServerSchemaProvider
{

    public class SqlServerProvider: DatabaseProvider
    {
        private IDatabase _database;

        public override IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = new SqlServerDatabase();
                }

                return this._database;
            }
        }

        public SqlServerProvider()
        {
            
        }

        public override IList<string> GetTableNames()
        {
            string sql =@"select t.name as TableName from sys.tables t order by t.name";

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
            string sql = @"select v.name as TableName from sys.views v order by v.name";
  
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
            return this.GetProcedureParameters().Select(p=>p.ProcedureName).ToList().Distinct().ToList();
        }

       
        public override IList<PrimaryKey> GetAllPrimaryKeys()
        {
            string sql = @"SELECT TableName=T.name,PrimaryKey=C.name FROM syscolumns C INNER JOIN " +
                "sysobjects T ON C.id=T.id and T.xtype='U' AND T.name<>'dtproperties' " +
                "WHERE EXISTS(SELECT 1 FROM sysobjects WHERE xtype='PK' AND name IN( " +
                "SELECT name FROM sysindexes WHERE indid IN( SELECT indid FROM sysindexkeys WHERE id=C.id AND colid=C.colid ))) ORDER BY T.name ";
  

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
            string sql = @"SELECT TableName=T.name,ForeignKey=FC.name,ReferenceTableName=R.name,ReferenceKey=RC.name FROM sysforeignkeys F LEFT JOIN " +
                    "sysobjects T ON F.fkeyid=T.id LEFT JOIN sysobjects R ON F.rkeyid=R.id LEFT JOIN " +
                    "syscolumns FC ON F.fkeyid=FC.id AND F.fkey=FC.colid LEFT JOIN syscolumns RC ON F.rkeyid=RC.id AND F.rkey=RC.colid " +
                    "WHERE T.xtype='U' AND R.xtype='U'";

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

        private Dictionary<string, string> GetIdentityDictionary()
        {
            string sql = @"select IdentityName=a.name,TableName=b.name  from  syscolumns  a
                        left join sysobjects b on a.id=b.id where COLUMNPROPERTY(a.id,a.name,'IsIdentity')=1";
   
            Dictionary<string, string> identityDictionary = new Dictionary<string, string>();
           
            DataTable identityTable = this.Database.ExecuteDataTable(sql, false);

                if (identityTable == null || identityTable.Columns.Count <= 0)
                {
                    return identityDictionary;
                }

                foreach (DataRow row in identityTable.Rows)
                {
                    if (!(row["IdentityName"] is DBNull))
                    {
                        identityDictionary.Add(row["TableName"].ToString(), row["IdentityName"].ToString());
                    }
                }
            
            return identityDictionary;
        }

        public override IList<IdentityKey> GetAllIdentityKeys()
        {
            string sql = @"select IdentityName=a.name,TableName=b.name  from  syscolumns  a
                        left join sysobjects b on a.id=b.id where COLUMNPROPERTY(a.id,a.name,'IsIdentity')=1";

            this.Database.Connection.Open();
            DataTable identityKeyTable = this.Database.ExecuteDataTable(sql, false);
            this.Database.Connection.Close();

            if (identityKeyTable == null || identityKeyTable.Rows.Count == 0)
            {
                return new List<IdentityKey>();
            }

            IList<IdentityKey> identityKeys = new List<IdentityKey>();
            IdentityKey identityKey = null;

            foreach (DataRow row in identityKeyTable.Rows)
            {
                identityKey = new IdentityKey();
                identityKey.TableName = row["TableName"].AsString();
                identityKey.Key = row["IdentityName"].AsString();

                if(!string.IsNullOrWhiteSpace(identityKey.TableName)||
                    !string.IsNullOrWhiteSpace(identityKey.Key))
                {
                    identityKeys.Add(identityKey);
                }
            }

            return identityKeys;
        }

        public override List<ProcedureParameter> GetProcedureParameters()
        {
            string sql = @"select a.name ProcedureName,c.name ParameterName,c.length Length,c.isoutparam IsOutput,d.name ParameterType " +
                    "from sysobjects a " +
                    "left join syscomments b on a.id = b.id " +
                    "left join syscolumns c on a.id = c.id " +
                    "left join systypes d on c.xtype = d.xtype " +
                    "where (a.type='P'or a.type='FN') and a.status<>1 and d.name<>'sysname' order by a.name asc";
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
                parameter.IsOutput = row["IsOutput"].ToString();
                procedureParameters.Add(parameter);
            }
            return procedureParameters;
        }
    }
}
