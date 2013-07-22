using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.MySqlSchemaProvider
{
    public class MySqlCodeProvider : CodeProvider
    {
        public override string MapTableClassName(string tableName)
        {
            tableName = FilterOwner(tableName);
            return BlockCaseToPascalCase(tableName);
        }

        public override string MapViewClassName(string tableName)
        {
            tableName = FilterOwner(tableName);
            return BlockCaseToPascalCase(tableName);
        }

        public override string MapProceduteClassName(string name)
        {
            name = FilterOwner(name);
            return BlockCaseToPascalCase(name);
        }

        public override string ToPropertyName(string parameterName)
        {
            FilterPrefix(ref parameterName);
            return BlockCaseToPascalCase(parameterName);
        }

        private void FilterPrefix(ref string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return;
            }

            string[] removedChars = new string[] { "PI_", "PO_", "P_", "X_" };
            foreach (string removedChar in removedChars)
            {
                if (parameterName.StartsWith(removedChar))
                {
                    parameterName = parameterName.Remove(0, removedChar.Length);
                    return;
                }
            }
        }
    }
}