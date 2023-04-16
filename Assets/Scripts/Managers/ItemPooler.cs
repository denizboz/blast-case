using System;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Utility;

namespace Managers
{
    public class ItemPooler : Manager
    {
        [SerializeField] private SpriteContainer m_spriteContainer;
        
        [SerializeField] private Cube[] m_cubes;
        [SerializeField] private Duck[] m_ducks;
        [SerializeField] private Rocket[] m_rockets;
        [SerializeField] private Balloon[] m_balloons;

        private static readonly Dictionary<Type, Queue<Item>> poolDictionary = new Dictionary<Type, Queue<Item>>(4);

        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemPooler>(this);
        }

        private void OnEnable()
        {
            CreatePool(m_cubes);
            CreatePool(m_ducks);
            CreatePool(m_rockets);
            CreatePool(m_balloons);
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
        
        private static void CreatePool<T>(T[] items) where T : Item
        {
            var pool = new Queue<T>(items.Length);

            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
                pool.Enqueue(item);
            }

            var type = typeof(T);
            poolDictionary.Add(type, pool as Queue<Item>);
        }
    }
}
