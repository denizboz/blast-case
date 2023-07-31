using System;
using System.Linq;
using Board.Cubes;
using CommonTools.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ItemTypeGenerator", menuName = "Item Type Generator")]
    public class ItemTypeGenerator : ContainerSO
    {
        [SerializeField] private ItemContainer m_itemContainer;
        [SerializeField] private ProbabilityDistribution m_distribution;

        private Type[] m_cubeTypes;
        private Type[] m_itemTypes;
        
        private int m_totalCount;
        
        public override void Initialize()
        {
            m_cubeTypes = m_itemContainer.ItemPrefabs.Where(item => item is Cube)
                .Select(cube => cube.Type).ToArray();
            
            m_totalCount = m_distribution.Occurrences.Sum(info => info.Count);
            m_itemTypes = new Type[m_totalCount];

            var index = 0;
            
            foreach (var occurrence in m_distribution.Occurrences)
            {
                for (int i = 0; i < occurrence.Count; i++)
                {
                    m_itemTypes[index] = occurrence.Item.Type;
                    index++;
                }
            }

            m_itemTypes.Shuffle();
        }

        public Type GetRandomItemType()
        {
            var random = Random.Range(0, m_totalCount);
            return m_itemTypes[random];
        }
        
        public Type GetRandomCubeType()
        {
            var random = Random.Range(0, m_cubeTypes.Length);
            return m_cubeTypes[random];
        }
    }
}