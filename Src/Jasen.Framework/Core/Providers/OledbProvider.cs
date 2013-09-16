using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;

namespace Jasen.Framework
{
    public class OledbProvider : DatabaseProvider
    {
        private ISqlGenerator _sqlBuilder; private IDatabase _database;

        public override ISqlGenerator SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new OledbSqlGenerator();
                }

                return this._sqlBuilder;
            }
        }
        public override IDatabase Database
        {
            get
            {
                if (this._database == null)
                {
                    this._database = new OracleDatabase();
                }

                return this._database;
            }
        }
        public OledbProvider()
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
