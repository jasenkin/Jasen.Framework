using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using Jasen.Framework.Core;
using System.Data.SqlClient;
using Jasen.Framework.Infrastructure;

namespace Jasen.Framework
{
    public class SqlServerProvider : DatabaseProvider
    {
        private ISqlGenerator _sqlBuilder; 
        private IDatabase _Database;

        public override ISqlGenerator SqlBuilder 
        { 
            get 
            {
                if (this._sqlBuilder == null)
                {
                    this._sqlBuilder = new SqlServerSqlGenerator();
                }

                return this._sqlBuilder;
            } 
        }

        public override IDatabase Database
        {
            get
            {
                if (this._Database == null)
                {
                    this._Database = new SqlServerDatabase();
                }

                return this._Database;
            }
        }

        public SqlServerProvider()
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
                    this.Database.BeginTransaction();
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

                    if (this.Database.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
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

                using (SqlBulkCopy bulk = new SqlBulkCopy(this.Database.Connection.ConnectionString))
                {
                    bulk.BatchSize = entities.Count;
                    bulk.DestinationTableName = tableName;
                    //bulk.ColumnMappings.Add("propertyName", "columnName");
                    
                    // convert propertyName to columnName, and  convert entity to datatable
                    DataTable dataTable = EntityTransfer.ToTable(entities, true);
                    bulk.WriteToServer(dataTable);
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return entities.Count;
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
