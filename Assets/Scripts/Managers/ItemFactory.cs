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
        [SerializeField] private ItemContainerSO m_itemContainer;
        [SerializeField] private Transform m_itemsParent;

        private Dictionary<Type, Queue<Item>> m_poolDictionary;

        private const int poolDictSize = 16;
        private const int itemPoolSize = 32;
        
        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            CreatePools();
            GameEventSystem.AddListener<ItemDestroyedEvent>(Return);
        }

        public Item Get(Type type)
        {
            var pool = m_poolDictionary[type];

            var item = pool.Dequeue();
            item.gameObject.SetActive(true);

            return item;
        }
        
        public T Get<T>() where T : Item
        {
            var type = typeof(T);
            var pool = m_poolDictionary[type];

            var item = (T)pool.Dequeue();
            item.gameObject.SetActive(true);

            return item;
        }
        
        private void Return(object returnedItem)
        {
            var item = (Item)returnedItem;
            var type = item.Type;
            var pool = m_poolDictionary[type];
            
            item.gameObject.SetActive(false);
            
            pool.Enqueue(item);
        }
        
        private void CreatePools()
        {
            m_poolDictionary = new Dictionary<Type, Queue<Item>>(poolDictSize);

            var itemPrefabs = m_itemContainer.ItemPrefabs;
            
            foreach (var prefab in itemPrefabs)
            {
                var itemType = prefab.GetType();
                var itemPool = new Queue<Item>(itemPoolSize);
                
                for (int i = 0; i < itemPoolSize; i++)
                {
                    var item = Instantiate(prefab, m_itemsParent);
                    itemPool.Enqueue(item);
                    item.gameObject.SetActive(false);
                }
                
                m_poolDictionary.Add(itemType, itemPool);
            }
        }
    }
}
