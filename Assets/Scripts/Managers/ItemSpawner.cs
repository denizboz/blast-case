using UnityEngine;
using Board;

namespace Managers
{
    public class ItemSpawner : Manager
    {
        private ItemPooler m_itemPooler;
        private GridManager m_gridManager;
        private BoardManager m_boardManager;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemSpawner>(this);
        }

        private void OnEnable()
        {
            m_itemPooler = dependencyContainer.Resolve<ItemPooler>();
            m_gridManager = dependencyContainer.Resolve<GridManager>();
            m_boardManager = dependencyContainer.Resolve<BoardManager>();
        }

        public Item Spawn<T>(Vector2Int finalPosition) where T : Item
        {
            var spawnPosWorld = m_gridManager.GetSpawnPosition(column: finalPosition.y);
            var finalPosWorld = m_gridManager.GetWorldPosition(gridPos: finalPosition);
            
            var item = m_itemPooler.Get<T>();
            
            item.SetWorldPosition(spawnPosWorld);
            item.SetGridPositionAndSorting(finalPosition);
            m_boardManager.AddItemToBoard(item);
            item.MoveTo(finalPosWorld);

            return item;
        }
    }
}
