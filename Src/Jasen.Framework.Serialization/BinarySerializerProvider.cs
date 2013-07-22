using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jasen.Framework.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinarySerializerProvider : ISerializerProvider
    {       
        /// <summary>
        /// 
        /// </summary>
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

                Stream stream = new FileStream(filePath, FileMode.Create);
                new BinaryFormatter().Serialize(stream, entity);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public object Clone()
        {
            object obj;

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Seek(0, 0);
                obj = formatter.Deserialize(ms);
                ms.Close();
            }

            return obj;
        }   


        public T Deserialize<T>(string filePath) where T : class
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                object obj = new BinaryFormatter().Deserialize(stream);
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
