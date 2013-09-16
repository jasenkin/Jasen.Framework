using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Jasen.Framework
{ 
    public delegate void SystemExceptionEventHandler(Exception ex); 
     
    public class ExceptionManager
    {
        public static event SystemExceptionEventHandler OnSystemException;

        public static bool ThrowException = true;
         
        internal static void RaiseExceptionEvent(Exception ex)
        {
            if (ExceptionManager.OnSystemException != null)
            {
                ExceptionManager.OnSystemException(ex);
            }

            if (ThrowException)
            {
                throw ex;
            }
        } 
    }
}
