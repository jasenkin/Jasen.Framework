using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.Serialization
{
    public class SerializerProviderFactory
    {
        public static ISerializerProvider Create(SerializerType serializerType = SerializerType.Binary)
        {
            switch (serializerType)
            {
                case SerializerType.Binary:
                    return new BinarySerializerProvider();
                case SerializerType.Soap:
                    return new SoapSerializerProvider();
                case SerializerType.Xml:
                    return new XmlSerializerProvider();
                default:
                    return new BinarySerializerProvider();
            }
        }
    }
}
