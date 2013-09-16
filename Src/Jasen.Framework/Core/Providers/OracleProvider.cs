using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;
using System.Data.OracleClient;

namespace Jasen.Framework
{
    public class OracleProvider : DatabaseProvider
    {
        private ISqlGenerator _sqlBuilder;
        private IDatabase _database;

        public override ISqlGenerator SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new OracleSqlGenerator();
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

        public OracleProvider()
        {       
        }

        internal override int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool useTransaction = false)
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            SqlCommandParameter parameter;
            int affectedRowCount = 0; 

            try
            {
                if (useTransaction)
                {
                    this.Database.BeginTransaction();
                }

                IdentityPrimaryKeyInfo identityInfo = IdentityUtility.GetIdentityInfo(typeof(T));

                foreach (T entity in entities)
                {
                    if (entity.ChangedPropertyNames == null || entity.ChangedPropertyNames.Count <= 0)
                    {
                        continue;
                    }

                    parameter = this.SqlBuilder.CreateSqlInsertCommandParameter(entity, entity.ChangedPropertyNames, identityInfo, returnIdentity);

                    if (identityInfo != null && !string.IsNullOrWhiteSpace(identityInfo.Name))
                    {
                        string commandText = this.SqlBuilder.SelectNextIdentity(typeof(T));
                        object identityValue = this.Database.ExecuteScalar(commandText, false);
                        identityInfo.PropertyInfo.SetValue(entity, identityValue, null);

                        var identityParameter = this.SqlBuilder.CreateColumnParameter(identityInfo.ColumnAttribute,
                            identityInfo.Name, identityValue);
                        parameter.Parameters.Add(identityParameter);
                    }

                    if (this.Database.ExecuteNonQuery(parameter.Sql, false, parameter.Parameters) > 0)
                    {
                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (useTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (useTransaction)
                {
                    this.Database.Rollback();
                }

                throw ex;
            }
        }


        /// <summary>
        /// to do...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="byIdentity"></param>
        /// <param name="commitTransaction"></param>
        /// <returns></returns>
        internal override int BetchUpdate<T>(IList<T> entities, bool byIdentity, bool commitTransaction)
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            try
            {
                if (commitTransaction)
                {
                    this.Database.BeginTransaction();
                }

                string tableName = AttributeUtility.GetTableName(typeof(T));

                if (string.IsNullOrEmpty(tableName))
                {
                    throw new ArgumentNullException(MsgResource.InvalidEntityConfig);
                }

                OracleCommandBuilder commandBuilder = new OracleCommandBuilder(this.Database.DataAdapter as OracleDataAdapter);
                this.Database.DataAdapter.InsertCommand = commandBuilder.GetInsertCommand();

                DataTable dataTable = EntityTransfer.ToTable(entities, true);
                dataTable.TableName = tableName;
                int affectedCount = this.Database.DataAdapter.Update(new DataSet());

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.Database.Rollback();
                }

                throw ex;
            }
        }
    }
}
