using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.CodeGenerator
{
    public class SqliteTypeConverter
    {

        public static string ToCSharpType(string dbType)
        {
            if (string.IsNullOrEmpty(dbType))
            {
                return "?";
            }

            dbType = dbType.ToUpper();
            string csType = "?";

            switch (dbType)
            {
                case "SMALLINT":
                    csType = "Int16";
                    break;
                case "INT":
                    csType = "int";
                    break;
                case "REAL":
                    csType = "float";
                    break;
                case "FLOAT":
                case "DOUBLE":
                case "NUMBER":
                    csType = "double";
                    break;
                case "MONEY":
                case "CURRENCY":
                case "DECIMAL":
                case "NUMERIC":
                    csType = "decimal";
                    break;
                case "BIT":
                case "YESNO":
                case "LOGICAL":
                case "BOOL":
                case "BOOLEAN":
                    csType = "bool";
                    break;
                case "TINYINT":
                    csType = "byte";
                    break;
                case "INTEGER":
                case "COUNTER":
                case "AUTOINCREMENT":
                case "IDENTITY":
                case "LONG":
                case "BIGINT":
                    csType = "Int64";
                    break;
                case "BINARY":
                case "VARBINARY":
                case "BLOB":
                case "IMAGE":
                case "GENERAL":
                case "OLEOBJECT":
                    csType = "byte[]";
                    break;
                case "VARCHAR":
                case "VARCHAR2":
                case "NVARCHAR":
                case "NVARCHAR2":
                case "MEMO":
                case "LONGTEXT":
                case "NOTE":
                case "TEXT":
                case "NTEXT":
                case "STRING":
                case "CHAR":
                case "NCHAR":
                    csType = "string";
                    break;
                case "DATETIME":
                case "SMALLDATE":
                case "TIMESTAMP":
                case "DATE":
                case "TIME":
                    csType = "DateTime?";
                    break;
                case "UNIQUEIDENTIFIER":
                case "GUID":
                    csType = "Guid";
                    break;
            }

            return csType;
        }
    }
}