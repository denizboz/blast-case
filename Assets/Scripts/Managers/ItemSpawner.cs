using System;
using Board;
using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Managers
{
    public class ItemSpawner
    {
        private readonly ItemFactory m_itemFactory;
        private readonly GridManager m_gridManager;
        
        public ItemSpawner()
        {
            m_itemFactory = DI.Resolve<ItemFactory>();
            m_gridManager = DI.Resolve<GridManager>();
        }

        public Item Spawn(Type type, Vector2Int finalPos, float spawnDelay = 0f)
        {
            var spawnPosWorld = m_gridManager.GetSpawnPosition(gridPos: finalPos);
            var finalPosWorld = m_gridManager.GetWorldPosition(gridPos: finalPos);

            var item = m_itemFactory.Get(type);

            item.SetWorldPosition(spawnPosWorld);
            item.SetGridPositionAndSorting(finalPos);

            item.MoveTo(finalPosWorld, spawnDelay);

            return item;
        }
    }
}
