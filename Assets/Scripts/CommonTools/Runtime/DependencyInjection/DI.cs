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
            dictionary.TryAdd(typeof(T), obj);
        }

        public static T Resolve<T>()
        {
            if (dictionary.TryGetValue(typeof(T), out object obj))
                return (T)obj;
            
            throw new Exception($"No {typeof(T)} reference in container.");
        }
    }
}
