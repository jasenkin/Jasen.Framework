using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.SchemaProvider
{
    public class AttributeUtility
    {
        public static ProviderAttribute GetProviderAttribute(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(ProviderAttribute), true);
            ProviderAttribute attribute = null;

            foreach (object attr in customAttributes)
            {
                attribute = attr as ProviderAttribute;
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
        }

        public static string GetProviderName(Type type)
        {
            var attribute = GetProviderAttribute(type);

            if (attribute == null || string.IsNullOrWhiteSpace(attribute.Name))
            {

                return type.Name;
            }

            return attribute.Name;
        }
    }
}
