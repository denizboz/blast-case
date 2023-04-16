using System;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Utility;

namespace Managers
{
    [DefaultExecutionOrder(-40)]
    public class ItemPooler : Manager
    {
        [SerializeField] private SpriteContainer m_spriteContainer;

        [SerializeField] private Item[] m_cubes;
        [SerializeField] private Item[] m_ducks;
        [SerializeField] private Item[] m_rockets;
        [SerializeField] private Item[] m_balloons;

        private static readonly Dictionary<Type, Queue<Item>> poolDictionary = new Dictionary<Type, Queue<Item>>(4);

        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemPooler>(this);
            
            CreatePool<Cube>(m_cubes);
            CreatePool<Duck>(m_ducks);
            CreatePool<Balloon>(m_balloons);
            
            // CreatePool(m_rockets);
        }

        public T Get<T>() where T : Item
        {
            var type = typeof(T);
            var pool = poolDictionary[type];

            var item = (T)pool.Dequeue();
            item.gameObject.SetActive(true);

            return item;
        }

        public void Return<T>(T item) where T : Item
        {
            var type = typeof(T);
            var pool = poolDictionary[type];
            
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }
        
        private static void CreatePool<T>(Item[] items) where T : Item
        {
            var pool = new Queue<Item>(items.Length);

            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
                pool.Enqueue(item);
            }

            var type = typeof(T);
            poolDictionary.Add(type, pool);
        }
    }
}
