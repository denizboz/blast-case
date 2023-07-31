using System;
using UnityEngine;
using Board;
using CommonTools.Runtime.DependencyInjection;

namespace Managers
{
    public class ItemSpawner : MonoBehaviour, IDependency
    {
        private ItemFactory m_itemFactory;
        private GridManager m_gridManager;
        private BoardManager m_boardManager;
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            m_itemFactory = DI.Resolve<ItemFactory>();
            m_gridManager = DI.Resolve<GridManager>();
            m_boardManager = DI.Resolve<BoardManager>();
        }

        public Item Spawn(Type type, Vector2Int finalPosition, bool wholeColumn)
        {
            var spawnPosWorld = m_gridManager.GetSpawnPosition(gridPos: finalPosition, wholeColumn);
            var finalPosWorld = m_gridManager.GetWorldPosition(gridPos: finalPosition);
            
            var item = m_itemFactory.Get(type);
            
            item.SetWorldPosition(spawnPosWorld);
            item.SetGridPositionAndSorting(finalPosition);
            
            item.MoveTo(finalPosWorld, toBottom: finalPosition.x == BoardManager.Bottom);

            return item;
        }
    }
}
