using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Jasen.Framework.CodeGenerator
{
    public class SqlServerConverter
    {

        public static string ToCSharpType(string dbType)
        {
            if (string.IsNullOrEmpty(dbType))
            {
                return "?";
            }
           
            dbType = dbType.ToLower();
            string csType = "?";

            switch (dbType)
            {
                case "bigint": 
                    csType = "Int64";
                    break;
                case "binary": 
                    csType = "byte[]";
                    break;
                case "bit": 
                    csType = "bool";
                    break;
                case "char": 
                    csType = "string";
                    break;
                case "datetime": 
                    csType = "DateTime?";
                    break;
                case "decimal": 
                    csType = "decimal";
                    break;
                case "float": 
                    csType = "double";
                    break;
                case "image": 
                    csType = "byte[]";
                    break;
                case "int": 
                    csType = "int";
                    break;
                case "money": 
                    csType = "decimal";
                    break;
                case "nchar": 
                    csType = "string";
                    break;
                case "ntext": 
                    csType = "string";
                    break;
                case "numeric": 
                    csType = "decimal";
                    break;
                case "nvarchar": 
                    csType = "string";
                    break;
                case "real": 
                    csType = "float";
                    break;
                case "smalldatetime": 
                    csType = "DateTime?";
                    break;
                case "smallint": 
                    csType = "Int16";
                    break;
                case "smallmoney": 
                    csType = "decimal";
                    break;
                case "sql_variant":
                    csType = "object";
                    break;
                case "text": 
                    csType = "string";
                    break;
                case "timestamp": 
                    csType = "byte[]";
                    break;
                case "tinyint": 
                    csType = "byte";
                    break;
                case "uniqueidentifier":
                    csType = "Guid";
                    break;
                case "varbinary": 
                    csType = "byte[]";
                    break;
                case "varchar": 
                    csType = "string";
                    break;
                default: 
                    csType = dbType;
                    break;
            }

            return csType;
        }
    }
}
