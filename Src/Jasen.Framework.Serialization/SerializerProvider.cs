using System.Collections.Generic;

namespace Jasen.Framework.Serialization
{
    public  interface ISerializerProvider
    {
        bool Serialize<T>(string filePath, T entity);

        T Deserialize<T>(string filePath) where T : class;
    }
}
