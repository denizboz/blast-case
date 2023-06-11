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

        [SerializeField] private GameObject m_allItems;
        
        [SerializeField] private Item[] m_cubes;
        [SerializeField] private Item[] m_ducks;
        [SerializeField] private Item[] m_balloons;
        
        [SerializeField] private Item[] m_rockets;

        private static readonly Dictionary<Type, Queue<Item>> m_poolDictionary = new Dictionary<Type, Queue<Item>>(16);

        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemPooler>(this);
            
            CreatePools();
        }

        public T Get<T>() where T : Item
        {
            var type = typeof(T);
            var pool = m_poolDictionary[type];

            var item = (T)pool.Dequeue();
            item.gameObject.SetActive(true);

            item.Setup(m_spriteContainer);

            return item;
        }
        
        public static void Return<T>(T item) where T : Item
        {
            var type = typeof(T);
            var pool = m_poolDictionary[type];
            
            item.gameObject.SetActive(false);
            item.SetSprite(null);
            
            pool.Enqueue(item);
        }
        
        private void CreatePools()
        {
            var items = m_allItems.GetComponentsInChildren<Item>();

            foreach (var item in items)
            {
                var type = item.GetType();

                if (!m_poolDictionary.ContainsKey(type))
                    m_poolDictionary.Add(type, new Queue<Item>());
                
                item.gameObject.SetActive(false);
                m_poolDictionary[type].Enqueue(item);
            }
        }
    }
}
