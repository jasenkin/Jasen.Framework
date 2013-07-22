using System;
using System.Collections.Generic;
using System.Data;
using Jasen.Framework.Reflection;
using Jasen.Framework.Resources;
using System.Globalization;
using Jasen.Framework.Core;
using System.Collections;
using Jasen.Framework.Attributes;
using System.Reflection;

namespace Jasen.Framework.Strategy
{
    public abstract class DatabaseStrategy
    {
        private readonly DatabaseConfig _databaseConfig;
        private ITableCommandExecutor _commandExecutor;

        protected DatabaseStrategy(DatabaseConfig databaseConfig)
        {
            this._databaseConfig = databaseConfig;
        }

        protected abstract ISqlBuilder SqlBuilder { get; }

        protected ITableCommandExecutor CommandExecutor
        {
            get
            {
                if (_commandExecutor == null)
                {
                    IDatabase database = DatabaseFactory.CreateInstance(_databaseConfig.DatabaseType);
                    database.ConnectionString = _databaseConfig.ConnectionString;
                    this._commandExecutor = new CommandExecutor(database);
                }

                return this._commandExecutor;
            }
        }

        internal virtual int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false)
            where T : TableExecutor<T>, new()
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            SqlCommandParameter parameter;
            int affectedRowCount = 0;

            try
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.BeginTransaction();
                }

                IdentityPrimaryKeyInfo identityInfo = IdentityUtility.GetIdentityInfo(typeof(T));

                foreach (T entity in entities)
                {
                    if (entity.ChangedPropertyNames == null || entity.ChangedPropertyNames.Count <= 0)
                    {
                        continue;
                    }

                    parameter = this.SqlBuilder.CreateSqlInsertCommandParameter(
                        entity, entity.ChangedPropertyNames, identityInfo, returnIdentity);

                    if (this.CommandExecutor.ExecuteNonQuery(parameter.Sql, false, parameter.Parameters) > 0)
                    {
                        if (returnIdentity && identityInfo != null)
                        {
                            string commandText = this.SqlBuilder.SelectCurrentIdentity(typeof(T));
                            object identityValue = this.CommandExecutor.ExecuteScalar(commandText, false);
                            identityInfo.PropertyInfo.SetValue(entity, identityValue, null);
                        }

                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.Rollback();
                }

                throw new CommandException(ex.Message, MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Update<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : TableExecutor<T>, new()
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            List<string> propertyNames;
            int affectedRowCount = 0;
            SqlCommandParameter commandParameter;

            try
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.BeginTransaction();
                }

                IdentityPrimaryKeyInfo keyInfo = GetKeyInfo<T>(byIdentity);

                if (keyInfo == null)
                {
                    throw new ArgumentException(MsgResource.InvalidEntityConfig);
                }

                foreach (T entity in entities)
                {
                    propertyNames = entity.ChangedPropertyNames;

                    if (propertyNames == null || propertyNames.Count <= 0)
                    {
                        continue;
                    }

                    commandParameter = this.SqlBuilder.CreateSqlUpdateCommandParameter(entity, keyInfo, byIdentity);

                    if (commandParameter == null || commandParameter.Parameters.Count <= 0)
                    {
                        continue;
                    }

                    if (this.CommandExecutor.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
                    {
                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedRowCount;
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

        private static IdentityPrimaryKeyInfo GetKeyInfo<T>(bool isIdentity)
        {
            if (isIdentity)
            {
                return IdentityUtility.GetIdentityInfo(typeof(T));
            }

            return KeyUtility.GetPrimaryKeyInfo(typeof(T));
        }

        internal int Update<T>(T entity, string condition, bool commitTransaction) where T : TableExecutor<T>, new()
        {
            if (entity == null || string.IsNullOrEmpty(condition))
            {
                return 0;
            }

            int affectedRowCount = 0;
            SqlCommandParameter commandParameter;

            try
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.BeginTransaction();
                }

                commandParameter = this.SqlBuilder.CreateSqlUpdateCommandParameter<T>(entity, condition);

                if (commandParameter == null || commandParameter.Parameters.Count <= 0)
                {
                    return 0;
                }

                if (this.CommandExecutor.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
                {
                    affectedRowCount++;
                    entity.ClearChangedPropertyNames();
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Delete<T>(IList<T> entities, bool byIdentity = false,
            bool commitTransaction = false) where T : TableExecutor<T>, new()
        {
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }

            string sql;
            int affectedRowCount = 0;

            try
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.BeginTransaction();
                }

                foreach (T entity in entities)
                {
                    sql = byIdentity ? this.SqlBuilder.DeleteById(entity) : this.SqlBuilder.DeleteByPK(entity);
                    if (this.CommandExecutor.ExecuteNonQuery(sql, false) > 0)
                    {
                        affectedRowCount++;
                    }
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Delete<T>(string condition, bool commitTransaction = false) where T : TableExecutor<T>, new()
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                return 0;
            }

            int affectedRowCount = 0;

            try
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.BeginTransaction();
                }

                string sql = this.SqlBuilder.Delete<T>(condition);

                if (this.CommandExecutor.ExecuteNonQuery(sql, false) > 0)
                {
                    affectedRowCount++;
                }

                if (commitTransaction)
                {
                    this.CommandExecutor.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.CommandExecutor.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Count(Type type, string condition)
        {
            string sql = this.SqlBuilder.Count(type, condition);
            object obj = this.CommandExecutor.ExecuteScalar(sql, false);

            return obj.AsInt();
        }

        internal double Sum(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Sum(type, columnName, condition);
            object obj = this.CommandExecutor.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal double Max(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Max(type, columnName, condition);
            object obj = this.CommandExecutor.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal double Min(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Min(type, columnName, condition);
            object obj = this.CommandExecutor.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal T RetrieveById<T>(T entity, bool byIdentity = false) where T : ViewExecutor<T>, new()
        {
            string sql = byIdentity ? this.SqlBuilder.SelectById(entity) : this.SqlBuilder.SelectByPK(entity);
            return RetrieveBySql<T>(sql);
        }

        internal T RetrieveBySql<T>(string sql) where T : ViewExecutor<T>, new()
        {
            DataTable dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.CommandExecutor.ExecuteDataTable(sql, false);

            if (dt != null && dt.Rows.Count > 0)
            {
                EntityMapper<T>.AdaptToEntity(dt);
                return EntityMapper<T>.ToEntity(dt.Rows[0]);
            }

            return default(T);
        }

        internal bool Exists<T>(T entity, bool byIdentity = false) where T : ViewExecutor<T>, new()
        {
            string sql = byIdentity ? this.SqlBuilder.ExistsById(entity) : this.SqlBuilder.ExistsByPK(entity);
            return ExistsBySql(sql);
        }

        internal bool Exists<T>(string condition) where T : ViewExecutor<T>, new()
        {
            if (string.IsNullOrEmpty(condition))
            {
                return false;
            }

            string sql = this.SqlBuilder.Exists(typeof(T), condition);
            return ExistsBySql(sql);
        }

        private bool ExistsBySql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return false;
            }

            int count = 0;
            object obj = this.CommandExecutor.ExecuteScalar(sql, false);

            if (obj != null && !(obj is DBNull))
            {
                count = Convert.ToInt32(obj, CultureInfo.InvariantCulture);
            }

            return count > 0;
        }

        internal DataTable QueryTop<T>(string condition, int topCount, Order order = null,
            params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            IList<string> propertyNameList = GetPropertyNameList(propertyNames);
            string sql = this.SqlBuilder.SelectTop(typeof(T), condition, topCount, order, propertyNameList);
            return QueryBySql<T>(sql);
        }

        internal DataTable Query<T>(string condition, Order order, params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            IList<string> propertyNameList = GetPropertyNameList(propertyNames);
            string sql = this.SqlBuilder.SelectByCondition(typeof(T), condition, order, propertyNameList);
            return QueryBySql<T>(sql);
        }

        internal DataTable Query<T>(string condition, PagingArg pagingArg, Order order = null,
            bool sqlServer2000 = false, params string[] propertyNames) where T : ViewExecutor<T>, new()
        {
            if (pagingArg == null || pagingArg.PageSize <= 0)
            {
                throw new ArgumentException("pagingArg is null.pagingArg.PageSize must be bigger than zero.");
            }

            int pageSize = pagingArg.PageSize;
            pagingArg.RowCount = Count(typeof(T), condition);
            pagingArg.PageCount = pagingArg.RowCount % pageSize == 0 ?
                (pagingArg.RowCount / pageSize) : (pagingArg.RowCount / pageSize + 1);
            int pageIndex = pagingArg.PageIndex > pagingArg.PageCount ? pagingArg.PageCount - 1 : pagingArg.PageIndex;
            pageIndex = pagingArg.PageIndex < 0 ? 0 : pagingArg.PageIndex;

            var propertyNameList = GetPropertyNameList(propertyNames);
            string sql = null;

            if (sqlServer2000)
            {
                sql = new SqlServerSqlBuilder().SelectPaging2000(typeof(T), condition, pageSize, pageIndex, order, propertyNameList);
            }
            else
            {
                sql = this.SqlBuilder.SelectPaging(typeof(T), condition, pageSize, pageIndex, order, propertyNameList);
            }

            return QueryBySql<T>(sql);
        }

        private static IList<string> GetPropertyNameList(IEnumerable<string> propertyNames)
        {
            var propertyNameList = new List<string>();

            if (propertyNames != null)
            {
                propertyNameList.AddRange(propertyNames);
            }

            return propertyNameList;
        }

        private DataTable QueryBySql<T>(string sql) where T : ViewExecutor<T>, new()
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.CommandExecutor.ExecuteDataTable(sql, false);
            EntityMapper<T>.AdaptToEntity(dt);
            return dt;
        }

        private DataTable QueryBySql(string sql, Type type)
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.CommandExecutor.ExecuteDataTable(sql, false);
            EntityMapper.AdaptToEntity(dt, type);
            return dt;
        }
 
        private void ExecuteOneToOneAssociation(AssociationAttribute currentAttribute,
            PropertyInfo currentProperty, object container, Type elementType)
        {
            string sql = this.SqlBuilder.CreateOneToOneAssociationSql(currentAttribute, elementType, container);

            QueryAndSetToOneResult(elementType, container, sql, currentProperty);
        }

        private void ExecuteOneToManyAssociation(AssociationAttribute currentAttribute,
            PropertyInfo currentProperty, object container, PropertyElement propertyElement)
        {
            string sql = this.SqlBuilder.CreateOneToManyAssociationSql(currentAttribute,
                propertyElement.ElementType, container);

            QueryAndSetToManyResult(propertyElement, container, sql, currentProperty);
        }

        private void ExecuteManyToOneAssociation(AssociationAttribute currentAttribute,
            PropertyInfo currentProperty, object container, Type elementType)
        {
            string sql = this.SqlBuilder.CreateManyToOneAssociationSql(currentAttribute, elementType, container);

            QueryAndSetToOneResult(elementType, container, sql, currentProperty);
        }

        private void QueryAndSetToOneResult(Type elementType, object container,
            string sql, PropertyInfo currentProperty)
        {
            DataTable dt = QueryBySql(sql, elementType);

            if (dt != null && dt.Rows.Count > 0)
            {
                object currValue = EntityMapper.ToEntity(dt.Rows[0], elementType);
                currentProperty.SetValue(container, currValue, null);
            }
        }

        private void ExecuteManyToManyAssociation(AssociationAttribute currentAttribute,
            PropertyInfo currentProperty, object container, PropertyElement propertyElement)
        {
            string sql = this.SqlBuilder.CreateManyToManyAssociationSql(currentAttribute,
                propertyElement.ElementType, container);
            QueryAndSetToManyResult(propertyElement, container, sql, currentProperty);
        }

        private void QueryAndSetToManyResult(PropertyElement propertyElement,
            object container, string sql, PropertyInfo currentProperty)
        {
            DataTable dt = QueryBySql(sql, propertyElement.ElementType);

            if (propertyElement.PropertyType == PropertyType.DataTable)
            {
                currentProperty.SetValue(container, dt, null);
                return;
            }

            if (propertyElement.PropertyType == PropertyType.Element)
            {
                var currValue = EntityMapper.ToEntities(dt, propertyElement.ElementType);
                currentProperty.SetValue(container, currValue, null);
                return;
            }

            object elementObj = Activator.CreateInstance(propertyElement.ElementType);
            MethodInfo methodInfo = null;

            if (propertyElement.PropertyType == PropertyType.Array)
            {
                methodInfo = elementObj.GetType().GetMethod("ToEntityArray");
            }
            else if (propertyElement.PropertyType == PropertyType.List)
            {
                methodInfo = elementObj.GetType().GetMethod("ToEntities");
            }
            else if (propertyElement.PropertyType == PropertyType.ArrayList)
            {
                methodInfo = elementObj.GetType().GetMethod("ToEntityArrayList");
            }

            object methodReturn = methodInfo.Invoke(elementObj, new object[] { dt });
            currentProperty.SetValue(container, methodReturn, null);
        }

        private static PropertyElement GetElementType(AssociationAttribute currentAttribute,
            PropertyInfo currentProperty)
        {
            var propertyElement = new PropertyElement();
            Type elementType = null;
          
            if (currentProperty.PropertyType.IsArray)
            {
                propertyElement.ElementType = currentProperty.PropertyType.GetElementType();
                propertyElement.PropertyType = PropertyType.Array;
                return propertyElement;
            }
            else if (currentProperty.PropertyType == typeof(DataTable))
            {
                elementType = currentAttribute.ElementType;

                if (elementType == null)
                {
                    throw new ArgumentException(@"While Entity Property With AssociationAttribute 's
                    propertyType is DataTable,AssociationAttribute must have ElementType.");
                }

                propertyElement.ElementType = elementType;
                propertyElement.PropertyType = PropertyType.DataTable;
                return propertyElement;
            }
            else if (currentProperty.PropertyType == typeof(ArrayList))
            {
                elementType = currentAttribute.ElementType;

                if (elementType == null)
                {
                    throw new ArgumentException(@"While Entity Property With AssociationAttribute 's
                    propertyType is DataTable,AssociationAttribute must have ElementType.");
                }

                propertyElement.ElementType = elementType;
                propertyElement.PropertyType = PropertyType.ArrayList;
                return propertyElement;
            }
            else if (currentProperty.PropertyType.TryListOfWhat(out elementType))
            {
                propertyElement.ElementType = elementType;
                propertyElement.PropertyType = PropertyType.List;

                return propertyElement;
            }
            else
            {
                propertyElement.ElementType = currentProperty.PropertyType;
                propertyElement.PropertyType = PropertyType.Element;
                return propertyElement;
            }
        }

        internal void RetrieveAssociation(object container, params string[] properties)
        {
            if (container == null)
            {
                return;
            }

            IList<PropertyAssociation> associations = GetAssociations(container, properties);

            if(associations==null||associations.Count==0)
            {
                return;
            }

            foreach(var association in associations)
            {
                RetrieveAssociation(container, association);
            }    
        }

        private static IList<PropertyAssociation> GetAssociations(object container, string[] properties)
        {
            IList<PropertyAssociation> associations = null;
            if(properties==null||properties.Length==0)
            {
                associations = AttributeUtility.GetAllPropertyAssociations(container.GetType());
            }
            else
            {
                associations = AttributeUtility.GetAllPropertyAssociations(container.GetType(), properties);
            }
            return associations;
        }

        private void RetrieveAssociation(object container, PropertyAssociation propertyAssociation)
        {
            if (propertyAssociation == null)
            {
                return;
            }

            PropertyInfo currentProperty = propertyAssociation.Property;
            AssociationAttribute currentAttribute = propertyAssociation.Association;
            AssociationType associationType = currentAttribute.AssociationType;
            PropertyElement propertyElement = GetElementType(currentAttribute, currentProperty);

            if (associationType == AssociationType.OneToOne)
            {
                if (propertyElement.PropertyType != PropertyType.Element)
                {
                    throw new ArgumentException("Invalid Property Type.");
                }

                ExecuteOneToOneAssociation(currentAttribute, currentProperty, container, propertyElement.ElementType);
            }
            else if (associationType == AssociationType.OneToMany)
            {
                ExecuteOneToManyAssociation(currentAttribute, currentProperty, container, propertyElement);
            }
            else if (associationType == AssociationType.ManyToOne)
            {
                if (propertyElement.PropertyType != PropertyType.Element)
                {
                    throw new ArgumentException("Invalid Property Type.");
                }

                ExecuteManyToOneAssociation(currentAttribute, currentProperty, container, propertyElement.ElementType);
            }
            else if (associationType == AssociationType.ManyToMany)
            {
                ExecuteManyToManyAssociation(currentAttribute, currentProperty, container, propertyElement);
            }
        }

        internal abstract int BetchUpdate<T>(IList<T> entities, bool byIdentity, bool commitTransaction) where T : TableExecutor<T>, new();
    }
}