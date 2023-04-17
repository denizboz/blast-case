using UnityEngine;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer SRenderer;
        
        public void SetSprite(Sprite sprite)
        {
            SRenderer.sprite = sprite;
        }
        
        public void SetPositionAndSorting(int x, int y)
        {
            Position = new Vector2Int(x, y);
            SRenderer.sortingOrder = x;
        }
    }
}
