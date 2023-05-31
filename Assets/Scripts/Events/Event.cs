using System;
using System.Collections.Generic;

namespace Events
{
    public class Event
    {
        private readonly List<Action<object>> m_actions = new List<Action<object>>();

        public void AddListener(Action<object> action)
        {
            m_actions.Add(action);
        }

        public void RemoveListener(Action<object> action)
        {
            m_actions.Remove(action);
        }

        public void Trigger(object item)
        {
            foreach (var action in m_actions)
            {
                action?.Invoke(item);
            }
        }
    }
}
