using System;
using UnityEngine;

namespace CommonTools.Runtime.TaskManagement
{
    public class Updater : MonoBehaviour
    {
        private static event Action onUpdate;

        static Updater()
        {
            var updater = new GameObject("Updater").AddComponent<Updater>();
            DontDestroyOnLoad(updater);
        }
        
        private void Update()
        {
            onUpdate?.Invoke();
        }

        public static void Subscribe(Action action)
        {
            onUpdate += action;
        }

        public static void Unsubscribe(Action action)
        {
            onUpdate -= action;
        }
    }
}
