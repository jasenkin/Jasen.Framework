using System;
using System.Collections.Generic; 
using System.Reflection;

namespace Jasen.Framework
{
    public class TableExecutor<T> : ViewExecutor<T>, ITableExecutor<T> where T : TableExecutor<T>, new()
    {
        private readonly List<string> _changedPropertyNames = new List<string>();

        public List<string> ChangedPropertyNames
        {
            get
            {
                return _changedPropertyNames;
            }
        }

        public bool Changed
        {
            get
            {
                return _changedPropertyNames.Count > 0;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (!_changedPropertyNames.Contains(propertyName))
            {
                _changedPropertyNames.Add(propertyName);
            }
        }

        protected void OnPropertyQuerying(MethodBase propertyMethod)
        {
            string methodPrefix = "get_";

            if (!propertyMethod.Name.StartsWith(methodPrefix))
            {
                return;
            }

            string propertyName = propertyMethod.Name.Remove(0, methodPrefix.Length);

            base.RetrieveAssociations(propertyName);      
        }

        protected void OnPropertyChanged(MethodBase propertyMethod)
        {
            string methodPrefix = "set_";

            if (!propertyMethod.Name.StartsWith(methodPrefix))
            {
                return;
            }

            OnPropertyChanged(propertyMethod.Name.Remove(0, methodPrefix.Length));
        }

        internal void ClearChangedPropertyNames()
        {
            _changedPropertyNames.Clear();
        }
         
        public int AddNew(bool returnIdentity = false, bool commitTransaction = false)
        {
            return AddNew(new List<T>() { this.Entity }, returnIdentity, commitTransaction);
        }

        public int AddNew(T entity, bool returnIdentity = false, bool commitTransaction = false)
        { 
            return AddNew(new List<T>() { entity }, returnIdentity, commitTransaction); 
        }

        public int AddNew(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false)
        {
            return this.DatabaseStrategy.AddNew(entities, returnIdentity, commitTransaction); 
        }
        
        public int DeleteById(T entity, bool commitTransaction = false)
        {
            if (entity == null)
            {
                return 0;
            }

            return DeleteById(new List<T>() {entity}, commitTransaction);
        }

        public int DeleteById(IList<T> entities, bool commitTransaction = false)
        {
            return DeleteByIdPK(entities, true, commitTransaction);
        }

        public int DeleteByPK(T entity, bool commitTransaction = false)
        {
            if (entity == null)
            {
                return 0;
            }

            return DeleteByPK(new List<T>() { entity }, commitTransaction);
        }

        public int DeleteByPK(IList<T> entities, bool commitTransaction = false)
        {
            return DeleteByIdPK(entities, false, commitTransaction);
        }

        public int Delete(string condition, bool commitTransaction = false)
        {
            return this.DatabaseStrategy.Delete<T>(condition, commitTransaction);
        }

        private int DeleteByIdPK(IList<T> entities, bool byIdentity, bool commitTransaction)
        {     
            if (entities == null || entities.Count <= 0)
            {
                return 0;
            }
            
            return this.DatabaseStrategy.Delete(entities, byIdentity, commitTransaction);
        }

        public int UpdateById(T entity, bool commitTransaction = false)
        {
            if (entity == null)
            {
                return 0;
            }

            return UpdateById(new List<T>() { entity }, commitTransaction);
        }

        public int UpdateById(IList<T> entities, bool commitTransaction = false)
        {
            return UpdateByIdPK(entities, true, commitTransaction);
        }

        public int UpdateByPK(T entity, bool commitTransaction = false)
        {
            if (entity == null)
            {
                return 0;
            }

            return UpdateByPK(new List<T>() { entity }, commitTransaction);
        }

        public int UpdateByPK(IList<T> entities, bool commitTransaction = false)
        {
            return UpdateByIdPK(entities, false, commitTransaction);
        }

        private int UpdateByIdPK(IList<T> entities, bool byIdentity, bool commitTransaction)
        {
            return this.DatabaseStrategy.Update(entities, byIdentity, commitTransaction); 
        }

    }
}
