using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools.Runtime.TaskManagement
{
    // Simple Task Management for Delayed Actions
    public class GameTask
    {
        private Action m_action;
        private float m_fireTime;
        
        private static int poolSize = 128;
        private static readonly Queue<GameTask> taskPool = new Queue<GameTask>(poolSize);

        private GameTask()
        {
            // Cannot be instantiated outside.
        }
        
        static GameTask()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var task = new GameTask();
                taskPool.Enqueue(task);
            }
        }

        #region TaskInstance
        public static GameTask Wait(float time)
        {
            var task = CreateNewTask();
            task.SetTime(Time.time + time);
            return task;
        }

        public GameTask Do(Action action)
        {
            m_action ??= action;
            return this;
        }

        public void Kill()
        {
            m_action = null;
            DisposeTask(this);
        }
        
        private void Update()
        {
            if (Time.time < m_fireTime)
                return;
            
            m_action?.Invoke();
            Kill();
        }

        private void SetTime(float time)
        {
            m_fireTime = time;
        }
        #endregion

        #region TaskManagement
        private static GameTask CreateNewTask()
        {
            EnsurePoolCapacity();

            var task = taskPool.Dequeue();
            Updater.Subscribe(task.Update);
            return task;
        }
        
        private static void DisposeTask(GameTask task)
        {
            if (taskPool.Contains(task))
                return;
            
            Updater.Unsubscribe(task.Update);
            taskPool.Enqueue(task);
        }
        
        private static void EnsurePoolCapacity()
        {
            if (taskPool.Count > 0)
                return;

            poolSize *= 2;

            for (int i = poolSize / 2; i < poolSize; i++)
            {
                var task = new GameTask();
                taskPool.Enqueue(task);
            }
            
            Debug.LogWarning($"Task pool capacity increased from {(poolSize / 2).ToString()} to {poolSize.ToString()}");
        }
        #endregion
    }
}
