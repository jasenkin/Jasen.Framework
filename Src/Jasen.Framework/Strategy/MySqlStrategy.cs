using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Strategy
{ 
    public class MySqlStrategy : DatabaseStrategy
    {
        private ISqlBuilder _sqlBuilder;

        protected override ISqlBuilder SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new MySqlSqlBuilder();
                }

                return this._sqlBuilder;
            }
        }

        public MySqlStrategy(DatabaseConfig databaseConfig)
            :base(databaseConfig)
        { 
        }

         
        /// <summary>
        /// just use transaction to update records one by one, finally comit the transaction.
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
