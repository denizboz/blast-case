using UnityEngine;
using Board;

namespace Managers
{
    public class ItemSpawner : Manager
    {
        private ItemPooler m_itemPooler;
        private GridManager m_gridManager;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<ItemSpawner>(this);
        }

        private void Start()
        {
            m_itemPooler = dependencyContainer.Resolve<ItemPooler>();
            m_gridManager = dependencyContainer.Resolve<GridManager>();
        }

        public Item Spawn<T>(Vector2Int finalPosition, bool wholeColumn) where T : Item
        {
            var spawnPosWorld = m_gridManager.GetSpawnPosition(gridPos: finalPosition, wholeColumn);
            var finalPosWorld = m_gridManager.GetWorldPosition(gridPos: finalPosition);
            
            var item = m_itemPooler.Get<T>();
            
            item.SetWorldPosition(spawnPosWorld);
            item.SetGridPositionAndSorting(finalPosition);
            
            var isDuckAndGoingToBottom = typeof(T) == typeof(Duck) && finalPosition.x == BoardManager.Bottom;
            
            item.MoveTo(finalPosWorld, isDuck: isDuckAndGoingToBottom);

            return item;
        }
    }
}
