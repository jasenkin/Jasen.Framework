using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;
using System.Data.SqlClient;

namespace Jasen.Framework.Strategy
{
    public class SqlServerStrategy : DatabaseStrategy
    {
        private ISqlBuilder _sqlBuilder;

        protected override ISqlBuilder SqlBuilder 
        { 
            get 
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new SqlServerSqlBuilder();
                }

                return this._sqlBuilder;
            } 
        }

        public SqlServerStrategy(DatabaseConfig databaseConfig)
            : base(databaseConfig)
        {
        }

        internal override int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool useTransaction = false)
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            SqlCommandParameter commandParameter;
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

                    commandParameter = this.SqlBuilder.CreateSqlInsertCommandParameter(
                        entity, entity.ChangedPropertyNames,identityInfo, returnIdentity);

                    if (base.CommandExecutor.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
                    {
                        if (returnIdentity && identityInfo != null)
                        {
                            IDataParameter parameter = commandParameter.Parameters.FirstOrDefault(p => string.Equals(identityInfo.Name, p.ParameterName));

                            if(parameter!=null)
                            {
                                identityInfo.PropertyInfo.SetValue(entity, parameter.Value, null);
                            }  
                        }

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

                using (SqlBulkCopy bulk = new SqlBulkCopy(base.CommandExecutor.Database.Connection.ConnectionString))
                {
                    bulk.BatchSize = entities.Count;
                    bulk.DestinationTableName = tableName;
                    //bulk.ColumnMappings.Add("propertyName", "columnName");
                    
                    // convert propertyName to columnName, and  convert entity to datatable
                    DataTable dataTable = EntityMapper.ToTable(entities, true);
                    bulk.WriteToServer(dataTable);
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return entities.Count;
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
