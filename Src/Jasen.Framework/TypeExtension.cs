﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework
{
    public static class TypeExtension
    {
        public static bool IsInherit(this Type type, Type parentType)
        {
            if(type==null||parentType==null)
            {
                return false;
            }

            if (type == parentType)
            {
                return true;
            }

            if (parentType.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == parentType)
            {
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == parentType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryListOfWhat(this Type type, out Type innerType)
        {
            innerType = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                innerType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    innerType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }
    }
}
