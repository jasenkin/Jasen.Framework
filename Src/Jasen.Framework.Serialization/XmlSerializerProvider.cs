using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Linq;
using System;

namespace Jasen.Framework.Serialization
{ 
    public class XmlSerializerProvider : ISerializerProvider
    {
        /// <summary>
        ///  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Serialize<T>(string filePath, T entity)
        {
            if (string.IsNullOrEmpty(filePath) || entity == null)
            {
                return false;
            }

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T), typeof(T).Name);
                Stream stream = new FileStream(filePath, FileMode.Create);
                xmlSerializer.Serialize(stream, entity);
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
                var xmlSerializer = new XmlSerializer(typeof(T), typeof(T).Name);
                Stream stream = new FileStream(filePath, FileMode.Open);
                object obj = xmlSerializer.Deserialize(stream);
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
