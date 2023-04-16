using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "DependencyContainer", menuName = "Dependency Container")]
    public class DependencyContainerSO : ScriptableObject
    {
        private readonly Dictionary<Type, object> m_systemsDictionary = new Dictionary<Type, object>(8);

        public void Bind<T>(T obj)
        {
            var type = typeof(T);

            if (m_systemsDictionary.ContainsKey(type))
                m_systemsDictionary[type] = obj;
            else
                m_systemsDictionary.Add(type, obj);
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            if (!m_systemsDictionary.ContainsKey(type))
                throw new Exception($"No {type} reference in container.");

            return (T)m_systemsDictionary[type];
        }
    }
}
