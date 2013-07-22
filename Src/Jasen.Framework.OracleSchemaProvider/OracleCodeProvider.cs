using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework.OracleSchemaProvider
{
    public class OracleCodeProvider : CodeProvider
    {
        public override string MapTableClassAttribute(string tableName)
        {
            string owner;
            string entityName = FilterOwner(tableName, out owner);
            if (string.IsNullOrEmpty(owner))
            {
                return "[Table(TableName = \"" + tableName +
                    "\",IdSequence=\"SEQ_" + tableName + "\")]";
            }
            else
            {
                return "[Table(TableName = \"" + tableName +
                    "\",IdSequence=\"" + owner + ".SEQ_" + entityName + "\")]";
            }
        }

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

        public virtual string ToPropertyName(string parameterName)
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
