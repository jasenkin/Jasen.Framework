using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Collections;
using System;

namespace Jasen.Framework.Serialization
{
    public class SoapSerializerProvider : ISerializerProvider
    {
        public bool Serialize<T>(string filePath, T entity) 
        {
            if (string.IsNullOrEmpty(filePath) || entity == null)
            {
                return false;
            }

            try
            {
                Stream stream = new FileStream(filePath, FileMode.Create);
                new SoapFormatter().Serialize(stream, entity);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Deserialize<T>(string filePath) where T:class
        {           
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                object obj = new SoapFormatter().Deserialize(stream);
                stream.Close();

                return obj as T;
            }
            catch
            {
                return null;
            }
        }
    }
}
