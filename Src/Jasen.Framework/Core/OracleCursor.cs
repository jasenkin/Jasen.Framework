using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using Jasen.Framework.Resources;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OracleCursor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IDataParameter CreateCursorParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentException(MsgResource.InvalidArguments, new ArgumentNullException("parameterName"));
            }

            OracleParameter tempOracleParameter = new OracleParameter(parameterName, OracleType.Cursor);
            tempOracleParameter.Direction = ParameterDirection.Output;
            return tempOracleParameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static IDataParameter CreateCursorParameter(string parameterName, object parameterValue, bool isNullable)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentException(MsgResource.InvalidArguments, new ArgumentNullException("parameterName"));
            }

            OracleParameter tempOracleParameter = new OracleParameter(parameterName, parameterValue);
            IDataParameter parameter = new OracleParameter(tempOracleParameter.ParameterName, OracleType.Cursor, tempOracleParameter.Size, ParameterDirection.Output,
                isNullable, 0, 0, tempOracleParameter.SourceColumn, tempOracleParameter.SourceVersion, tempOracleParameter.Value);
            return parameter;
        }
    }
}
