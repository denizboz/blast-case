using System;
using System.Collections.Generic;

namespace Events
{
    public static class GameEventSystem
    {
        private static readonly Dictionary<Type, Event> eventTypeDictionary = new();

        public static void AddListener<T>(Action<object> action) where T : Event, new()
        {
            var type = typeof(T);
            var hasType = eventTypeDictionary.TryGetValue(type, out Event evt);

            if (!hasType)
            {
                evt = new T();
                eventTypeDictionary.Add(type, evt);
            }
            
            evt.AddListener(action);
        }

        public static void RemoveListener<T>(Action<object> action) where T : Event
        {
            var hasType = eventTypeDictionary.TryGetValue(typeof(T), out Event evt);

            if (!hasType) return;

            evt.RemoveListener(action);
        }
        
        public static void Invoke<T>(object obj = null) where T : Event
        {
            var type = typeof(T);
            var hasType = eventTypeDictionary.TryGetValue(type, out Event evt);

            if (!hasType) return;
            
            evt.Trigger(obj);
        }
    }
}
