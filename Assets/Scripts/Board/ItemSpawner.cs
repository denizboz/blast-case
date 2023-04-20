using Managers;
using UnityEngine;

namespace Board
{
    public class ItemSpawner
    {
        private ItemPooler m_itemPooler;
        private Vector2Int m_position;

        public void RegisterPooler(ItemPooler pooler)
        {
            m_itemPooler = pooler;
        }
        
        public void SetPosition(Vector2Int pos)
        {
            m_position = pos;
        }

        public void SpawnCube(CubeType type, Vector2Int fallPosition)
        {
            var cube = m_itemPooler.Get<Cube>();
            cube.SetType(type);
            
        }
    }
}