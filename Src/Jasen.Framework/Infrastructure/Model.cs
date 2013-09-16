using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jasen.Framework.Infrastructure
{
    public class Model : ITable
    {
        private readonly List<string> _changedPropertyNames = new List<string>();

        public List<string> ChangedPropertyNames
        {
            get
            {
                return this._changedPropertyNames;
            }
        }

        public bool Changed
        {
            get
            {
                return this._changedPropertyNames.Count > 0;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (!this._changedPropertyNames.Contains(propertyName))
            {
                this._changedPropertyNames.Add(propertyName);
            }
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

        public void ClearChangedPropertyNames()
        {
            this._changedPropertyNames.Clear();
        }
         
    }
}
