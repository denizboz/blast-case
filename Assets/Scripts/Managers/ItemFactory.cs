using System;
using System.Collections.Generic;
using UnityEngine;
using Board;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Utilities;

namespace Managers
{
    public class ItemFactory : MonoBehaviour, IDependency
    {
        [SerializeField] private SpriteContainer m_spriteContainer;
        [SerializeField] private GameObject m_allItems;
        
        private readonly Dictionary<Type, Queue<Item>> m_poolDictionary = new Dictionary<Type, Queue<Item>>(16);

        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            CreatePools();
            GameEventSystem.AddListener<ItemDestroyedEvent>(Return);
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
        
        private void Return(object returnedItem)
        {
            var item = (Item)returnedItem;
            var type = item.GetType();
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
