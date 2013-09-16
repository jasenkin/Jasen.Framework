using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{ 
    public class MySqlProvider : DatabaseProvider
    {
        private ISqlGenerator _sqlBuilder;
        private IDatabase _database;

        public override ISqlGenerator SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new MySqlGenerator();
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
        public MySqlProvider()
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
