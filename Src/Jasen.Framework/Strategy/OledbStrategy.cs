using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;

namespace Jasen.Framework.Strategy
{
    public class OledbStrategy : DatabaseStrategy
    {
        private ISqlBuilder _sqlBuilder;

        protected override ISqlBuilder SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new OledbSqlBuilder();
                }

                return this._sqlBuilder;
            }
        }

        public OledbStrategy(DatabaseConfig databaseConfig)
            : base(databaseConfig)
        {
             
        }


        /// <summary>
        /// Access can use transaction to betch update .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="byIdentity"></param>
        /// <param name="commitTransaction"></param>
        /// <returns></returns>
        internal override int BetchUpdate<T>(IList<T> entities, bool byIdentity, bool commitTransaction)
        {
            return base.Update<T>(entities, byIdentity, true);
        }
    }
}
