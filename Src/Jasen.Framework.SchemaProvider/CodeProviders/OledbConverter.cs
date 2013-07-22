using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.CodeGenerator
{
    public class OledbConverter
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
                case "2":
                    csType = "Int16";
                    break;
                case "3":
                    csType = "int";
                    break;
                case "4":
                    csType = "float";
                    break;
                case "5":
                    csType = "double";
                    break;
                case "6":
                    csType = "decimal";
                    break;
                case "7":
                    csType = "DateTime?";
                    break;
                case "11":
                    csType = "bool";
                    break;
                case "17":
                    csType = "byte";
                    break;
                case "72":
                    csType = "Guid";
                    break;
                case "128":
                    csType = "byte[]";
                    break;
                case "130":
                    csType = "string";
                    break;
                case "131":
                    csType = "decimal";
                    break;
                default: 
                    csType = dbType;
                    break;
            }

            return csType;
        }
     
    }
}
