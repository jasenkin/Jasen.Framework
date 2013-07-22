using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Data;
using Jasen.Framework.CodeGenerator;
using Jasen.Framework.MetaData;

namespace Jasen.Framework.SqliteSchemaProvider
{
    public class SqliteProvider : DatabaseProvider
    {
        private const string INTERGE = "INTEGER";

        private IDatabase _database;
        private bool _isColumnsInited = false;

        public override IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = new SqliteDatabase();
                }

                return this._database;
            }
        }

        public SqliteProvider()
        {
            
        }

        public IEnumerable<TableColumn> TableColumns { get; protected set; }

        public override void Init()
        {
            if (!this._isColumnsInited)
            {
                this.TableColumns = GetAllColumns();
            }

            base.Init();
        }

        public override IList<string> GetTableNames()
        {
            this.Database.Connection.Open();
            DataTable table = this.Database.ExecuteDataTable(
                    "SELECT [tbl_name] AS Name FROM  main.sqlite_master  WHERE [tbl_name] <>'sqlite_sequence' AND [type] LIKE 'table'", false);
            this.Database.Connection.Close();

            return ConvertToNames(table);
        }

        public override IList<string> GetViewNames()
        {
            this.Database.Connection.Open();
            DataTable table = this.Database.ExecuteDataTable(
               "SELECT [tbl_name] AS Name FROM  main.sqlite_master  WHERE [type] LIKE 'view'", false);
            this.Database.Connection.Close();

            return ConvertToNames(table);
        }

        public override IList<string> GetProcedureNames()
        {
            return new List<string>();
        }

        public override IEnumerable<TableColumn> GetTableOrViewInfo(string tableNameOrViewName)
        {
            if (string.IsNullOrWhiteSpace(tableNameOrViewName))
            {
                return new List<TableColumn>();
            }

            if (!this.IsInited)
            {
                this.Init();
            }

            var tableColumns = this.TableColumns.Where(item=>string.Equals(item.TableName,tableNameOrViewName.Trim()));
            ForeignKey foreignKey;
            IList<TableColumn> returnColumns = new List<TableColumn>();
            TableColumn tempColumn;

            foreach (TableColumn tableColumn in tableColumns)
            { 
                tempColumn = new TableColumn();
                tempColumn.TableName = tableNameOrViewName;
                tempColumn.ColumnName = tableColumn.ColumnName;
                tempColumn.DataType = SqliteTypeConverter.ToCSharpType(tableColumn.DataType);

                tempColumn.IsPrimaryKey = this.PrimaryKeys.Any(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, tableColumn.ColumnName));
                tempColumn.IsIdentity = this.IdentityKeys.Any(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, tableColumn.ColumnName));
                foreignKey = this.ForeignKeys.FirstOrDefault(item => string.Equals(item.TableName, tableNameOrViewName)
                    && string.Equals(item.Key, tableColumn.ColumnName));

                if (foreignKey != null)
                {
                    tempColumn.IsForeignKey = true;
                    tempColumn.ReferenceTableName = foreignKey.ReferenceTableName;
                    tempColumn.ReferenceKey = foreignKey.ReferenceKey;
                } 

                returnColumns.Add(tempColumn);
            }

            return returnColumns;
        }
          
        public override IList<PrimaryKey> GetAllPrimaryKeys()
        {
            IEnumerable<TableColumn> tableColumns = GetAllTableColumns();
            IEnumerable<TableColumn> primaryColumns = tableColumns.Where(p => p.IsPrimaryKey);

            if(primaryColumns.Count()==0)
            {
                return new List<PrimaryKey>();
            }

            var primaryKeys = new List<PrimaryKey>();
            PrimaryKey primaryKey;

            foreach(var column in primaryColumns)
            {
                primaryKey=new PrimaryKey();
                primaryKey.TableName = column.TableName;
                primaryKey.Key = column.ColumnName;

                primaryKeys.Add(primaryKey);
            }

            return primaryKeys;
        }

        public override IList<ForeignKey> GetAllForeignKeys()
        {
            this.Database.Connection.Open();
            DataTable foreignKeyTable = (this.Database.Connection as SQLiteConnection).GetSchema("FOREIGNKEYS");
            this.Database.Connection.Close();

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
                foreignKey.Key = row["FKEY_FROM_COLUMN"].AsString();
                foreignKey.ReferenceTableName = row["FKEY_TO_TABLE"].AsString();
                foreignKey.ReferenceKey = row["FKEY_TO_COLUMN"].AsString();
                foreignKeys.Add(foreignKey);
            }

            return foreignKeys;
        }

        public override IList<IdentityKey> GetAllIdentityKeys()
        {

            IEnumerable<TableColumn> tableColumns = GetAllTableColumns();

            if (tableColumns == null || tableColumns.Count() == 0)
            {
                return new List<IdentityKey>();
            }

            IList<IdentityKey> identityKeys = new List<IdentityKey>();
            IdentityKey identityKey = null;

            foreach (var tableColumn in tableColumns)
            {
                if (tableColumn.IsPrimaryKey && string.Equals(tableColumn.DataType.Trim().ToUpper(), INTERGE))
                {
                    identityKey = new IdentityKey();
                    identityKey.TableName = tableColumn.TableName.Trim();
                    identityKey.Key = tableColumn.ColumnName.Trim();

                    identityKeys.Add(identityKey);
                } 
            }

            return identityKeys;
        }

        private IEnumerable<TableColumn> GetAllTableColumns()
        {
            if (this._isColumnsInited)
            {
                return this.TableColumns ?? new List<TableColumn>();
            }

            return GetAllColumns();
        }

        private static IList<string> ConvertToNames(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return new List<string>();
            }

            var names = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                names.Add(row["Name"].AsString());
            }

            return names;
        }

        private IEnumerable<TableColumn> GetAllColumns()
        { 
            this.Database.Connection.Open();
            DataTable table = (this.Database.Connection as SQLiteConnection).GetSchema("TABLECOLUMNS");
            this.Database.Connection.Close();
            this._isColumnsInited = true;
            return ConvertToColumns(table);
        }

        private static IEnumerable<TableColumn> ConvertToColumns(DataTable columnsTable)
        {
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
                tempColumn.IsPrimaryKey = row["PRIMARY_KEY"].AsBool();
                tableColumns.Add(tempColumn);
            }

            return tableColumns;
        }
         
    }
}
