using System;
using System.Collections.Generic;

namespace CommonTools.Runtime.DependencyInjection
{
    /// <summary>
    /// Simple Dependency Injector for general use, allowing one dependency per type.
    /// </summary>
    public static class DI
    {
        private static readonly Dictionary<Type, object> dictionary = new Dictionary<Type, object>(64);

        public static void Bind<T>(T obj)
        {
            var type = typeof(T);
            
            if (dictionary.ContainsKey(type))
                dictionary[type] = obj;
            else
                dictionary.Add(type, obj);
        }

        public static T Resolve<T>()
        {
            var type = typeof(T);

            if (!dictionary.ContainsKey(type))
                throw new Exception($"No {type} reference in container.");

            return (T)dictionary[type];
        }
    }
}
