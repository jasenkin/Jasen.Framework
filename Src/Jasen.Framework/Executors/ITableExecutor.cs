using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Jasen.Framework.Core;

namespace Jasen.Framework
{
    public interface ITableExecutor<T> : IViewExecutor<T> where T : ITableExecutor<T>, new()
    {
        int AddNew(T entity, bool returnIdentity = false, bool commitTransaction = false);
        int AddNew(IList<T> entities, bool returnIdentity = false, bool commitTransaction = false);

        int DeleteById(T entity, bool commitTransaction = false);
        int DeleteByPK(T entity, bool commitTransaction = false);
        int Delete(string condition, bool commitTransaction = false);

        int UpdateById(T entity, bool commitTransaction = false);
        int UpdateByPK(T entity, bool commitTransaction = false);
    }
}
