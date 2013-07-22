using System;
using System.Collections.Generic; 
using Jasen.Framework.MetaData;
using Jasen.Framework.SchemaProvider;

namespace Jasen.Framework
{
    public interface IDatabaseProvider
    {
        bool IsInited { get; }
        IDatabase Database { get; }
        IList<string> TableNames { get;}
        IList<string> ViewNames { get;}
        IList<string> ProcedureNames { get; }
        IList<PrimaryKey> PrimaryKeys { get; }
        IList<ForeignKey> ForeignKeys { get;  }
        IList<IdentityKey> IdentityKeys { get; }
        CodeProvider CodeProvider { get; }

        void Init();
        IEnumerable<TableColumn> GetTableOrViewInfo(string tableNameOrViewName);
        IEnumerable<ProcedureParameter> GetProcedureParameters(string procedureName);
        IList<string> GetTableNames();
        IList<string> GetViewNames();
        IList<string> GetProcedureNames(); 
        IList<PrimaryKey> GetAllPrimaryKeys();
        IList<ForeignKey> GetAllForeignKeys();
        IList<IdentityKey> GetAllIdentityKeys();
        List<ProcedureParameter> GetProcedureParameters();
         
        string Build(string name, OperationType operationType, string nameSpace, bool addAttribute, string templateFilePath);
    }
}
