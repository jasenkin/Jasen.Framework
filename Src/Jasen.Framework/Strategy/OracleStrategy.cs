using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;
using System.Data.OracleClient;

namespace Jasen.Framework.Strategy
{
    public class OracleStrategy : DatabaseStrategy
    {
        private ISqlBuilder _sqlBuilder;

        protected override ISqlBuilder SqlBuilder
        {
            get
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new OracleSqlBuilder();
                }

                return this._sqlBuilder;
            }
        }

        public OracleStrategy(DatabaseConfig databaseConfig)
            : base(databaseConfig)
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
                    base.CommandExecutor.BeginTransaction();
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
                        object identityValue = base.CommandExecutor.ExecuteScalar(commandText, false);
                        identityInfo.PropertyInfo.SetValue(entity, identityValue, null);

                        var identityParameter = this.SqlBuilder.CreateColumnParameter(identityInfo.ColumnAttribute,
                            identityInfo.Name, identityValue);
                        parameter.Parameters.Add(identityParameter);
                    }

                    if (base.CommandExecutor.ExecuteNonQuery(parameter.Sql, false, parameter.Parameters) > 0)
                    {
                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (useTransaction)
                {
                    base.CommandExecutor.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (useTransaction)
                {
                    base.CommandExecutor.Rollback();
                }

                throw new CommandException(ex.Message, MsgResource.ExecuteNonQueryFailed, ex);
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
                    base.CommandExecutor.BeginTransaction();
                }

                string tableName = AttributeUtility.GetTableName(typeof(T));

                if (string.IsNullOrEmpty(tableName))
                {
                    throw new ArgumentNullException(MsgResource.InvalidEntityConfig);
                }

                OracleCommandBuilder commandBuilder = new OracleCommandBuilder(this.CommandExecutor.Database.DataAdapter as OracleDataAdapter);
                this.CommandExecutor.Database.DataAdapter.InsertCommand = commandBuilder.GetInsertCommand();

                DataTable dataTable = EntityMapper.ToTable(entities, true);
                dataTable.TableName = tableName;
                int affectedCount = this.CommandExecutor.Database.DataAdapter.Update(new DataSet());

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.Rollback();
                }

                throw new CommandException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }
    }
}
