using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Jasen.Framework.MetaData;

namespace Jasen.Framework.SchemaProvider
{ 
    public class CodeProvider  
    {
        public virtual string MapTableClassAttribute(string name)
        {
            return "[Table(TableName = \"" + name + "\")]";
        }

        public virtual string MapViewClassAttribute(string name)
        {
            return "[Table(TableName = \"" + name + "\")]";
        }

        public virtual string MapProceduteClassAttribute(string name)
        {
            return "[Procedure(Name = \"" + name + "\")]";
        }

        public virtual string MapTableClassName(string name)
        {
            return name;
        }

        public virtual string MapViewClassName(string name)
        {
            return name;
        }

        public virtual string MapProceduteClassName(string name)
        {
            return name;
        } 

        public virtual string ToFieldName(string columnName)
        {
            string fieldName = PascalCaseToCamelCase(ToPropertyName(columnName));

            if(string.Equals(fieldName,"ID",StringComparison.CurrentCultureIgnoreCase))
            {
                fieldName = "id";
            }

            return "_" + fieldName;
        }

        public virtual string ToPropertyName(string columnName)
        {
            return ToPascalCase(columnName);
        }
          
        public string MapMemberVariablesBlock(IEnumerable<TableColumn> columns)
        {
            var content = new StringBuilder();
            if (columns.Count() <= 0)
            {
                return content.ToString();
            }
            content.Append("\n");
            foreach (TableColumn column in columns)
            {
                content.Append(CreateTabSpaces(2) + "private " + column.DataType + " " + this.ToFieldName(column.ColumnName) + ";\n");
            }
            return content.ToString();
        } 

        public string MapPropertiesBlock(IEnumerable<TableColumn> columns, bool addAttribute)
        {
            StringBuilder content = new StringBuilder();
            if (columns.Count() <= 0)
            {
                return content.ToString();
            }

            content.Append("\n");
            foreach (TableColumn column in columns)
            { 
                if (addAttribute)
                {
                    AppendPropertyAttributes(content, column);
                }

                AppendProperty(addAttribute, content, column.DataType, column.ColumnName);
            }

            return content.ToString();
        }

        public virtual string MapMemberVariablesBlock(IEnumerable<ProcedureParameter> parameters)
        {
            StringBuilder content = new StringBuilder();
            if (parameters.Count() <= 0)
            {
                return content.ToString();
            }
            content.Append("\n");
            foreach (ProcedureParameter parameter in parameters)
            {
                string parameterName = parameter.ParameterName;

                if (string.IsNullOrEmpty(parameter.ParameterName))
                {
                    parameterName = EmptyDataEntity;
                }

                content.Append(CreateTabSpaces(2) + "private " + parameter.ParameterType + " " +
                    this.ToFieldName(parameterName) + ";\n");
            }
            return content.ToString();

        }

        public virtual string MapPropertiesBlock(IEnumerable<ProcedureParameter> parameters)
        {
            var content = new StringBuilder();

            if (parameters.Count() <= 0)
            {
                return content.ToString();
            }

            content.Append("\n");
            foreach (ProcedureParameter parameter in parameters)
            {
                string parameterName = parameter.ParameterName;

                if (string.IsNullOrEmpty(parameter.ParameterName))
                {
                    parameterName = EmptyDataEntity;
                }

                string direction = (parameter.IsOutput == "1") ? "Direction=ParameterDirection.Output" : "Direction=ParameterDirection.Input";
                content.Append(CreateTabSpaces(2));
                content.Append("[Parameter(Name = \"" + parameterName + "\"," + direction + ")]\n");

                AppendProperty(false, content, parameter.ParameterType, parameterName);
            } 

            return content.ToString();
        }
         
        public virtual void AppendPropertyAttributes(StringBuilder content, TableColumn column)
        {
            content.Append(CreateTabSpaces(2));
            if (column.IsIdentity && column.IsPrimaryKey)
            {
                content.Append("[Column(ColumnName = \"" + column.ColumnName + "\",IsIdentity = true,IsPrimaryKey = true)]\n");
            }
            else if (column.IsIdentity)
            {
                content.Append("[Column(ColumnName = \"" + column.ColumnName + "\",IsIdentity = true)]\n");
            }
            else if (column.IsPrimaryKey)
            {
                content.Append("[Column(ColumnName = \"" + column.ColumnName + "\",IsPrimaryKey = true)]\n");
            }
            else if (column.IsForeignKey)
            {
                content.Append("[Column(ColumnName = \"" + column.ColumnName + "\",IsForeignKey = true,ReferenceType=typeof(" +
                    column.ReferenceTableName + "))]\n");
            }
            else
            {
                content.Append("[Column(ColumnName = \"" + column.ColumnName + "\")]\n");
            }
        }

        private void AppendProperty(bool addAttribute, StringBuilder content, string dataType, string parameterName)
        {
            string propertyName = this.ToPropertyName(parameterName);
            string fieldName = this.ToFieldName(parameterName);

            content.Append(CreateTabSpaces(2) + "public " + dataType + " " + propertyName + "\n");
            content.Append(CreateTabSpaces(2) + "{\n");
            content.Append(CreateTabSpaces(3) + "get\n");
            content.Append(CreateTabSpaces(3) + "{\n");
            content.Append(CreateTabSpaces(4) + "return " + fieldName + ";\n");
            content.Append(CreateTabSpaces(3) + "}\n");
            content.Append(CreateTabSpaces(3) + "set\n");
            content.Append(CreateTabSpaces(3) + "{\n");
            content.Append(CreateTabSpaces(4) + fieldName + " = value;\n");
            if (addAttribute)
            {
                content.Append(CreateTabSpaces(4) + "base.OnPropertyChanged(MethodInfo.GetCurrentMethod());\n");
            }
            content.Append(CreateTabSpaces(3) + "}\n");
            content.Append(CreateTabSpaces(2) + "}\n\n");
        }

        public string BuildProcedure(string name, string nameSpace, 
            string templateFilePath, IEnumerable<ProcedureParameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException(templateFilePath);
            }

            var content = new StringBuilder();
            content.Append(File.ReadAllText(templateFilePath));
            BuildClassAttribute(content, true, name, OperationType.Procedure);
            BuildNameSpace(content, nameSpace);
            content.Replace("@MemberVariables", this.MapMemberVariablesBlock(parameters));
            content.Replace("@Properties", this.MapPropertiesBlock(parameters));
            content.Replace("@ClassName", this.MapProceduteClassName(name.Trim()));

            return content.ToString();
        }

        private void BuildNameSpace(StringBuilder content, string nameSpace)
        {
            content.Replace("@NameSpace", nameSpace);
        }

        internal string BuildTable(string name, string nameSpace, bool addAttribute, string templateFilePath, IEnumerable<TableColumn> columns)
        {
            var content = new StringBuilder();
            content.Append(File.ReadAllText(templateFilePath));
            BuildClassAttribute(content, addAttribute, name, OperationType.Table);
            BuildNameSpace(content, nameSpace);
            content.Replace("@MemberVariables", this.MapMemberVariablesBlock(columns));
            content.Replace("@Properties", this.MapPropertiesBlock(columns, addAttribute));
            content.Replace("@ClassName", this.MapTableClassName(name.Trim()));
            return content.ToString();
        }

        internal string BuildView(string name, string nameSpace, bool addAttribute, string templateFilePath, IEnumerable<TableColumn> columns)
        {
            var content = new StringBuilder();
            content.Append(File.ReadAllText(templateFilePath));
            BuildClassAttribute(content, addAttribute, name, OperationType.View);
            BuildNameSpace(content, nameSpace);
            content.Replace("@MemberVariables", this.MapMemberVariablesBlock(columns));
            content.Replace("@Properties", this.MapPropertiesBlock(columns, addAttribute));
            content.Replace("@ClassName", this.MapViewClassName(name.Trim()));
            return content.ToString();
        }

        private void BuildClassAttribute(StringBuilder content, bool addAttribute, string name,OperationType operationType)
        {
            string classAttribute = string.Empty;

            if(addAttribute)
            {
                if (operationType == OperationType.Procedure)
                {
                    classAttribute = this.MapProceduteClassAttribute(name);
                }
                else if (operationType == OperationType.View)
                {
                    classAttribute = this.MapViewClassAttribute(name);
                }
                else
                {
                    classAttribute = this.MapTableClassAttribute(name);
                }
            }

            content.Replace("@TableAttribute", classAttribute);
        }
  
        public static readonly string EmptyDataEntity = "Result";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pascalCamelString"></param>
        /// <returns></returns>
        public static string ToPascalCase(string pascalCamelString)
        {
            if (string.IsNullOrEmpty(pascalCamelString))
            {
                return EmptyDataEntity;
            }

            if (pascalCamelString.Length <= 1)
            {
                return pascalCamelString;
            }

            if (pascalCamelString.Length == 2)
            {
                return ProcessTwoCharPart(pascalCamelString);
            }

            string varName = pascalCamelString.Substring(0, 1).ToUpper() + pascalCamelString.Substring(1);

            return varName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pascalCaseString"></param>
        /// <returns></returns>
        public static string PascalCaseToCamelCase(string pascalCaseString)
        {
            if (string.IsNullOrEmpty(pascalCaseString))
            {
                return EmptyDataEntity;
            }

            if (pascalCaseString.Length<=1)
            {
                return pascalCaseString;
            }

            if (pascalCaseString.Length == 2)
            {
                return ProcessTwoCharPart(pascalCaseString);
            }

            string varName = "";
            if (pascalCaseString.Substring(0, 2).ToUpper() == pascalCaseString.Substring(0, 2))
            {
                varName = pascalCaseString;
            }
            else
            {
                varName = pascalCaseString.Substring(0, 1).ToLower() + pascalCaseString.Substring(1);
            }
            return varName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockCaseString"></param>
        /// <returns></returns>
        public static string BlockCaseToCamelCase(string blockCaseString)
        {
            string varName = BlockCaseToPascalCase(blockCaseString);
            if (varName.Length > 2)
            {
                varName = varName.Substring(0, 1).ToLower() + varName.Substring(1);
            }
            return varName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockCaseString"></param>
        /// <returns></returns>
        public static string BlockCaseToPascalCase(string blockCaseString)
        {
            if (string.IsNullOrEmpty(blockCaseString))
            {
                return EmptyDataEntity;
            }
            string[] stringParts = blockCaseString.Split('_','.');
 
            string varName = "";
            foreach (string stringPart in stringParts)
            {
                varName += ProcessStringPart(stringPart);
            }
            return varName;
        }

        private static string ProcessStringPart(string stringPart)
        {
            if (string.IsNullOrEmpty(stringPart) || stringPart.Length<=1)
            {
                return stringPart;
            }

            if (stringPart.Length == 2)
            {
                return ProcessTwoCharPart(stringPart);
            }

            return stringPart.Substring(0, 1).ToUpper() + stringPart.Substring(1).ToLower();
        }

        private static string ProcessTwoCharPart(string stringPart)
        {
            stringPart = stringPart.ToUpper();
            if (stringPart == "ID")
            {
                return "Id";
            }

            if (stringPart == "IS")
            {
                return "Is";
            }

            if (stringPart == "BY")
            {
                return "By";
            }

            if (stringPart == "TO")
            {
                return "To";
            }

            return stringPart;
        }

        /// <summary>
        /// replaces \t with 4 spaces
        /// </summary>
        /// <returns></returns>
        public static string CreateTabSpaces()
        {
            return "    ";
        }

        public static string CreateTabSpaces(int count)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                str.Append(CreateTabSpaces());
            }
            return str.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullEntityName"></param>
        /// <returns></returns>
        public static string FilterOwner(string fullEntityName)
        {
             string owner;
             return FilterOwner(fullEntityName, out owner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FilterOwner(string fullEntityName,out string owner)
        {
            owner = "";
            if (string.IsNullOrEmpty(fullEntityName))
            {
                return fullEntityName;
            }

            string entityName = fullEntityName;
            int indexOfDot = fullEntityName.LastIndexOf('.');
            
            if (indexOfDot > 0 && fullEntityName.Length > (indexOfDot + 1))
            {
                entityName = fullEntityName.Substring(indexOfDot + 1);
                owner = fullEntityName.Substring(0,indexOfDot);
            }
            return entityName;
        }
    }
}
