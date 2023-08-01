using System;
using System.Linq;
using Board.Cubes;
using CommonTools.Runtime;

namespace Utilities
{
    public class ItemTypeGenerator
    {
        private readonly Type[] m_cubeTypes;
        private readonly Type[] m_itemTypes;

        private readonly Random m_random;
        private readonly int m_totalCount;
        
        public ItemTypeGenerator(ItemContainerSO itemContainer, ProbabilityDistributionSO distribution)
        {
            m_random = new Random();
            
            m_cubeTypes = itemContainer.ItemPrefabs.Where(item => item is Cube)
                .Select(cube => cube.Type).ToArray();
            
            m_totalCount = distribution.Occurrences.Sum(info => info.Count);
            m_itemTypes = new Type[m_totalCount];

            var index = 0;
            
            foreach (var occurrence in distribution.Occurrences)
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
            var random = m_random.Next(0, m_totalCount);
            return m_itemTypes[random];
        }
        
        public Type GetRandomCubeType()
        {
            var random = m_random.Next(0, m_cubeTypes.Length);
            return m_cubeTypes[random];
        }
    }
}