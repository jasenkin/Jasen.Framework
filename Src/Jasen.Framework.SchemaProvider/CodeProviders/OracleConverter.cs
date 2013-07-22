using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.CodeGenerator
{
    public class OracleConverter
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
                case "BLOB": csType = "byte[]";
                    break;
                case "CLOB": csType = "string";
                    break;
                case "CHAR": csType = "string";
                    break;
                case "DATE": csType = "DateTime?";
                    break;
                case "LONG": csType = "string";
                    break;
                case "NCLOB": csType = "string";
                    break;
                case "NUMBER": csType = "decimal";
                    break;
                case "NVARCHAR2": csType = "string";
                    break;
                case "RAW": csType = "byte[]";
                    break;
                case "VARCHAR2": csType = "string";
                    break;
                case "TIMESTAMP": csType = "DateTime?";
                    break;
                case "REF CURSOR": csType = "OracleCursor";
                    break;
                default: csType = dbType;
                    break;
            }

            return csType;
        }
    }
}
