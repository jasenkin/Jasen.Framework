using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.MetaData
{
    public class ProcedureParameter
    {
        public string ProcedureName { get; set; }
        public string ParameterName { get; set; }
        public string Length { get; set; }
        public string ParameterType { get; set; }
        public string IsOutput { get; set; }
    }
}
