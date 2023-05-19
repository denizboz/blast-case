using System;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    [DefaultExecutionOrder(-40)]
    public class ItemPooler : Manager
    {
        [SerializeField] private SpriteContainer m_spriteContainer;
        
        [SerializeField] private Item[] m_cubes;
        [SerializeField] private Item[] m_ducks;
        [SerializeField] private Item[] m_balloons;
        
        [SerializeField] private Item[] m_rockets;

        private static readonly Dictionary<Type, Queue<Item>> m_poolDictionary = new Dictionary<Type, Queue<Item>>(4);

        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemPooler>(this);
            
            CreatePool<Cube>(m_cubes);
            CreatePool<Duck>(m_ducks);
            CreatePool<Balloon>(m_balloons);
            
            CreatePool<Rocket>(m_rockets);
        }

        public T Get<T>() where T : Item
        {
            var type = typeof(T);
            var pool = m_poolDictionary[type];

            var item = (T)pool.Dequeue();
            item.gameObject.SetActive(true);

            Sprite sprite;
            
            if (item is Cube cube)
            {
                var cubeType = (CubeType)Random.Range(0, Cube.VarietySize);
                sprite = m_spriteContainer.GetSprite<Cube>(cubeType);
                
                cube.SetType(cubeType);
            }
            else if (item is Rocket rocket)
            {
                var rocketType = (RocketType)Random.Range(0, Rocket.VarietySize);
                sprite = null;
                
                rocket.SetType(rocketType);
            }
            else
            {
                sprite = m_spriteContainer.GetSprite<T>();
            }

            item.SetSprite(sprite);
            
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
        
        private static void CreatePool<T>(Item[] items) where T : Item
        {
            var pool = new Queue<Item>(items.Length);

            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
                pool.Enqueue(item);
            }

            var type = typeof(T);
            m_poolDictionary.Add(type, pool);
        }
    }
}
