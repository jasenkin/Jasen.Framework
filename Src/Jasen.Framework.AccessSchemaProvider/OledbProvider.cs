using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Jasen.Framework.MetaData;
using Jasen.Framework.CodeGenerator;

namespace Jasen.Framework.AccessSchemaProvider
{ 
    public class OledbProvider : DatabaseProvider
    {
        private IDatabase _database;

        public override IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = new OleDatabase();
                }

                return this._database;
            }
        }

        public OledbProvider()
        {
            
        }

        public override IList<string> GetTableNames()
        {
            this.Database.Connection.Open();
            DataTable table = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
            this.Database.Connection.Close();

            return ConvertToNames(table, "TABLE_NAME");
        }

        public override IList<string> GetViewNames()
        {
            this.Database.Connection.Open();
            DataTable table = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Views, null);
            this.Database.Connection.Close();

            return ConvertToNames(table, "TABLE_NAME");
        }

        public override IList<string> GetProcedureNames()
        {
            this.Database.Connection.Open();
            DataTable table = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, null);
            this.Database.Connection.Close();

            return ConvertToNames(table, "PROCEDURE_NAME");
        } 
        
        public override IList<PrimaryKey> GetAllPrimaryKeys()
        {
            this.Database.Connection.Open();
            DataTable primaryKeyTable = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(
                OleDbSchemaGuid.Primary_Keys, null);
            this.Database.Connection.Close();

            if (primaryKeyTable == null || primaryKeyTable.Rows.Count == 0)
            {
                return new List<PrimaryKey>();
            }

            IList<PrimaryKey> primaryKeys = new List<PrimaryKey>();
            PrimaryKey primaryKey;

            foreach (DataRow row in primaryKeyTable.Rows)
            {
                primaryKey = new PrimaryKey();
                primaryKey.TableName = row["TABLE_NAME"].AsString();
                primaryKey.Key = row["COLUMN_NAME"].AsString(); 
                primaryKeys.Add(primaryKey);
            }

            return primaryKeys;
        }

        public override IList<ForeignKey> GetAllForeignKeys()
        {
            this.Database.Connection.Open();
            DataTable foreignKeyTable = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(
                OleDbSchemaGuid.Foreign_Keys, null);
            this.Database.Connection.Close();

            if(foreignKeyTable==null||foreignKeyTable.Rows.Count==0)
            {
                return new List<ForeignKey>();
            }

            IList<ForeignKey> foreignKeys = new List<ForeignKey>();
            ForeignKey foreignKey = null;

            foreach (DataRow row in foreignKeyTable.Rows)
            {
                foreignKey = new ForeignKey();
                foreignKey.TableName = row["PK_TABLE_NAME"].AsString();
                foreignKey.Key = row["PK_COLUMN_NAME"].AsString();
                foreignKey.ReferenceTableName = row["FK_TABLE_NAME"].AsString();
                foreignKey.ReferenceKey = row["FK_COLUMN_NAME"].AsString();
                foreignKeys.Add(foreignKey);
            }

            return foreignKeys;
        }

        public override IList<IdentityKey> GetAllIdentityKeys()
        {
            var tableColumns = GetAllColumns();

            if (tableColumns == null || tableColumns.Count() == 0)
            {
                return new List<IdentityKey>();
            }

            IList<IdentityKey> identityKeys = new List<IdentityKey>();
            IdentityKey identityKey = null;

            foreach (var tableColumn in tableColumns)
            {
                if (tableColumn.ColumnFlag == 90 && string.Equals(tableColumn.DataType.Trim(), "3"))
                {
                    identityKey = new IdentityKey();
                    identityKey.TableName = tableColumn.TableName.Trim();
                    identityKey.Key = tableColumn.ColumnName.Trim();

                    identityKeys.Add(identityKey);
                }
            }

            return identityKeys;
        }

        protected override string CreateSelectCommandText(string tableNameOrViewName)
        {
            return "select * from [" + tableNameOrViewName.Trim() + "] where null = null";
        }

        private static IList<string> ConvertToNames(DataTable table, string columnName)
        {
            if (table == null || table.Rows.Count == 0 
                || !table.Columns.Contains(columnName))
            {
                return new List<string>();
            }

            var names = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                names.Add(dataRow[columnName].AsString());
            }

            return names;
        }

        private IEnumerable<TableColumn> GetAllColumns()
        {
            this.Database.Connection.Open();
            DataTable columnsTable = (this.Database.Connection as OleDbConnection).GetOleDbSchemaTable(
                OleDbSchemaGuid.Columns, null);
            this.Database.Connection.Close();

            if (columnsTable == null || columnsTable.Rows.Count == 0)
            {
                return new List<TableColumn>();
            }

            IList<TableColumn> tableColumns = new List<TableColumn>();
            TableColumn tempColumn = null;

            foreach (DataRow row in columnsTable.Rows)
            {
                tempColumn = new TableColumn();
                tempColumn.TableName = (row["TABLE_NAME"] ?? string.Empty).ToString().Trim();
                tempColumn.ColumnName = (row["COLUMN_NAME"] ?? string.Empty).ToString().Trim();
                tempColumn.DataType = (row["DATA_TYPE"] ?? string.Empty).ToString().Trim(); 
                tempColumn.ColumnFlag = row["COLUMN_FLAGS"].AsInt(); // INT类型90为自增长
                tableColumns.Add(tempColumn);
            }

            return tableColumns;
        }
         
        public override string GetDataType(string databaseType)
        {
            return OledbConverter.ToCSharpType(databaseType);
        } 
    }
}
