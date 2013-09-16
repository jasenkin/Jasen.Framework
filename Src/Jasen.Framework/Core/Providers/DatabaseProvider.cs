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
using System.Linq;
using Jasen.Framework.Infrastructure;

namespace Jasen.Framework
{
    public abstract class DatabaseProvider
    {  
        public DatabaseProvider()
        { 
        }
          
        public abstract IDatabase Database { get;  }

        public abstract ISqlGenerator SqlBuilder { get; } 

        internal virtual int AddNew<T>(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false)
            where T : ITable, new()
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
                    this.Database.BeginTransaction(null);
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

                    if (this.Database.ExecuteNonQuery(parameter.Sql, false, parameter.Parameters) > 0)
                    {
                        if (returnIdentity && identityInfo != null)
                        {
                            string commandText = this.SqlBuilder.SelectCurrentIdentity(typeof(T));
                            object identityValue = this.Database.ExecuteScalar(commandText, false);
                            identityInfo.PropertyInfo.SetValue(entity, identityValue, null);
                        }

                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
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

        internal int Update<T>(IList<T> entities, bool byIdentity = false, bool commitTransaction = false)
            where T : ITable, new()
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
                    this.Database.BeginTransaction();
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

                    if (this.Database.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
                    {
                        affectedRowCount++;
                        entity.ClearChangedPropertyNames();
                    }
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
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

        private static IdentityPrimaryKeyInfo GetKeyInfo<T>(bool isIdentity)
        {
            if (isIdentity)
            {
                return IdentityUtility.GetIdentityInfo(typeof(T));
            }

            return KeyUtility.GetPrimaryKeyInfo(typeof(T));
        }

        internal int Update<T>(T entity, string condition, bool commitTransaction) where T : ITable, new()
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
                    this.Database.BeginTransaction();
                }

                commandParameter = this.SqlBuilder.CreateSqlUpdateCommandParameter<T>(entity, condition);

                if (commandParameter == null || commandParameter.Parameters.Count <= 0)
                {
                    return 0;
                }

                if (this.Database.ExecuteNonQuery(commandParameter.Sql, false, commandParameter.Parameters) > 0)
                {
                    affectedRowCount++;
                    entity.ClearChangedPropertyNames();
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.Database.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Delete<T>(IList<T> entities, bool byIdentity = false,
            bool commitTransaction = false) where T : ITable, new()
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
                    this.Database.BeginTransaction();
                }

                foreach (T entity in entities)
                {
                    sql = byIdentity ? this.SqlBuilder.DeleteById(entity) : this.SqlBuilder.DeleteByPK(entity);
                    if (this.Database.ExecuteNonQuery(sql, false) > 0)
                    {
                        affectedRowCount++;
                    }
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.Database.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Delete<T>(string condition, bool commitTransaction = false) where T : ITable, new()
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
                    this.Database.BeginTransaction();
                }

                string sql = this.SqlBuilder.Delete<T>(condition);

                if (this.Database.ExecuteNonQuery(sql, false) > 0)
                {
                    affectedRowCount++;
                }

                if (commitTransaction)
                {
                    this.Database.Commit();
                }

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                if (commitTransaction)
                {
                    this.Database.Rollback();
                }

                throw new ArgumentException(MsgResource.ExecuteNonQueryFailed, ex);
            }
        }

        internal int Count(Type type, string condition)
        {
            string sql = this.SqlBuilder.Count(type, condition);
            object obj = this.Database.ExecuteScalar(sql, false);

            return obj.AsInt();
        }

        internal double Sum(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Sum(type, columnName, condition);
            object obj = this.Database.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal double Max(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Max(type, columnName, condition);
            object obj = this.Database.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal double Min(Type type, string columnName, string condition)
        {
            string sql = this.SqlBuilder.Min(type, columnName, condition);
            object obj = this.Database.ExecuteScalar(sql, false);

            return obj.AsDouble();
        }

        internal T RetrieveById<T>(T entity, bool byIdentity = false) where T : IView, new()
        {
            string sql = byIdentity ? this.SqlBuilder.SelectById(entity) : this.SqlBuilder.SelectByPK(entity);
            return RetrieveBySql<T>(sql);
        }

        internal T RetrieveBySql<T>(string sql) where T : IView, new()
        {
            DataTable dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.Database.ExecuteDataTable(sql, false);

            if (dt != null && dt.Rows.Count > 0)
            {
                EntityTransfer<T>.AdaptToEntity(dt);
                return EntityTransfer<T>.ToEntity(dt.Rows[0]);
            }

            return default(T);
        }

        internal bool Exists<T>(T entity, bool byIdentity = false) where T : IView, new()
        {
            string sql = byIdentity ? this.SqlBuilder.ExistsById(entity) : this.SqlBuilder.ExistsByPK(entity);
            return ExistsBySql(sql);
        }

        internal bool Exists<T>(string condition) where T : IView, new()
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
            object obj = this.Database.ExecuteScalar(sql, false);

            if (obj != null && !(obj is DBNull))
            {
                count = Convert.ToInt32(obj, CultureInfo.InvariantCulture);
            }

            return count > 0;
        }

        internal DataTable QueryTop<T>(string condition, int topCount, Order order = null,
            params string[] propertyNames) where T : IView, new()
        {
            IList<string> propertyNameList = GetPropertyNameList(propertyNames);
            string sql = this.SqlBuilder.SelectTop(typeof(T), condition, topCount, order, propertyNameList);
            return QueryBySql<T>(sql);
        }

        internal DataTable Query<T>(string condition, Order order, params string[] propertyNames) where T : IView, new()
        {
            IList<string> propertyNameList = GetPropertyNameList(propertyNames);
            string sql = this.SqlBuilder.SelectByCondition(typeof(T), condition, order, propertyNameList);
            return QueryBySql<T>(sql);
        }

        internal DataTable Query<T>(string condition, PagingArg pagingArg, Order order = null, params string[] propertyNames) where T : IView, new()
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
            string sql = this.SqlBuilder.SelectPaging(typeof(T), condition, pageSize, pageIndex, order, propertyNameList);

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

        private DataTable QueryBySql<T>(string sql) where T : IView, new()
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.Database.ExecuteDataTable(sql, false);
            EntityTransfer<T>.AdaptToEntity(dt);
            return dt;
        }

        private DataTable QueryBySql(string sql, Type type)
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            dt = this.Database.ExecuteDataTable(sql, false);
            EntityTransfer.AdaptToEntity(dt, type);
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
                object currValue = EntityTransfer.ToEntity(dt.Rows[0], elementType);
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
                var currValue = EntityTransfer.ToEntities(dt, propertyElement.ElementType);
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

        internal abstract int BetchUpdate<T>(IList<T> entities, bool byIdentity, bool commitTransaction) where T : ITable, new();

        public bool BeginTransaction()
        {
            return this.Database.BeginTransaction() != null;
        }

        public bool Commit()
        {
            return this.Database.Commit();
        }

        public bool Rollback()
        {
            return this.Database.Rollback();
        }

        public int ExecuteNonQuery(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            return this.Database.ExecuteNonQuery(commandText, isProcedure, parameters);
        }

        public int ExecuteNonQuery<T>(T entity) where T : class, IStoreProcedure
        {
            string procedureName = GetProcedureName<T>();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, entity, this.SqlBuilder);
            int count = ExecuteNonQuery(procedureName, true, dataParameters);
            UpdateOutputProperties(entity, dictionary, dataParameters);

            return count;
        }

        public object ExecuteScalar(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            return this.Database.ExecuteScalar(commandText, isProcedure, parameters);
        }

        public object ExecuteScalar<T>(T entity) where T : class, IStoreProcedure
        {
            string procedureName = GetProcedureName<T>();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, entity, this.SqlBuilder);
            object obj = ExecuteScalar(procedureName, true, dataParameters);
            UpdateOutputProperties(entity, dictionary, dataParameters);

            return obj;
        }

        public IDataReader ExecuteReader(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            return this.Database.ExecuteReader(commandText, isProcedure, parameters);
        }
         
        public IDataReader ExecuteReader<T>(T entity) where T : class, IStoreProcedure
        {
            string procedureName = GetProcedureName<T>();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, entity, this.SqlBuilder);
            IDataReader reader = ExecuteReader(procedureName, true, dataParameters);
            UpdateOutputProperties(entity, dictionary, dataParameters);

            return reader;
        }

        public void CloseReader(IDataReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }

            if (this.Database.ConnectionState != ConnectionState.Closed)
            {
                this.Database.Close();
            }
        }

        public DataTable ExecuteDataTable(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            return this.Database.ExecuteDataTable(commandText, isProcedure, parameters);
        }

        public DataTable ExecuteDataTable<T>(T entity) where T : class, IStoreProcedure
        {
            string procedureName = GetProcedureName<T>();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, entity, this.SqlBuilder);
            DataTable dt = ExecuteDataTable(procedureName, true, dataParameters);
            UpdateOutputProperties(entity, dictionary, dataParameters);

            return dt;
        }

        public DataSet ExecuteDataSet(string commandText, bool isProcedure = false, IList<IDataParameter> parameters = null)
        {
            return this.Database.ExecuteDataSet(commandText, isProcedure, parameters);
        }

        public DataSet ExecuteDataSet<T>(T entity) where T : class, IStoreProcedure
        {
            string procedureName = GetProcedureName<T>();
            var dictionary = AttributeUtility.GetParameterAndProperties(typeof(T));
            var dataParameters = ToDataParameters(dictionary, entity, this.SqlBuilder);
            DataSet ds = ExecuteDataSet(procedureName, true, dataParameters);

            UpdateOutputProperties(entity, dictionary, dataParameters);
            return ds;
        }

        private static string GetProcedureName<T>() where T : class, IStoreProcedure
        {
            var procedureAttribute = AttributeUtility.GetProcedureAttribute(typeof(T));

            if (procedureAttribute == null)
            {
                throw new ArgumentException(MsgResource.InvalidEntityConfig);
            }

            return string.IsNullOrEmpty(procedureAttribute.Name) ? typeof(T).Name : procedureAttribute.Name;
        }

        private void UpdateOutputProperties<T>(T entity, IDictionary<ParameterAttribute, PropertyInfo> parameterAttributes,
            IEnumerable<IDataParameter> dataParameters) where T : class, IStoreProcedure
        {
            foreach (IDataParameter parameter in dataParameters)
            {
                if (parameter.Direction == ParameterDirection.Input)
                {
                    continue;
                }

                ParameterAttribute parameterAttr = parameterAttributes.Keys.FirstOrDefault(p => string.Equals(p.Name, parameter.ParameterName));

                if (parameterAttr == null)
                {
                    continue;
                }

                if (parameterAttributes[parameterAttr].PropertyType == typeof(OracleCursor))
                {
                    continue;
                }

                parameterAttributes[parameterAttr].SetValue(entity, parameter.Value, null);
            }
        }

        private IList<IDataParameter> ToDataParameters<T>(IDictionary<ParameterAttribute, PropertyInfo> dictionary,
             T entity, ISqlGenerator sqlBuilder) where T : class, IStoreProcedure
        {
            if (dictionary == null || dictionary.Keys.Count == 0)
            {
                return new List<IDataParameter>();
            }

            var dataParameters = new List<IDataParameter>();

            IDataParameter dataParameter;
            string name;
            object value;

            foreach (var parameterKey in dictionary.Keys)
            {
                name = string.IsNullOrEmpty(parameterKey.Name) ? dictionary[parameterKey].Name : parameterKey.Name;
                value = dictionary[parameterKey].GetValue(entity, null);

                dataParameter = sqlBuilder.CreateProcedureParameter(parameterKey, name, value);

                if (dataParameter != null)
                {
                    dataParameters.Add(dataParameter);
                }
            }

            return dataParameters;
        }
    }
}