using DG.Tweening;
using UnityEngine;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer SRenderer;

        private const float fallSpeed = 7f;
        
        public void SetSprite(Sprite sprite)
        {
            SRenderer.sprite = sprite;
        }
        
        public void SetGridPositionAndSorting(Vector2Int gridPosition)
        {
            Position = gridPosition;
            SRenderer.sortingOrder = gridPosition.x;
        }

        public void SetWorldPosition(Vector3 worldPos)
        {
            transform.position = worldPos;
        }
        
        public void MoveTo(Vector3 pos, float speed = fallSpeed)
        {
            var distance = Vector3.Distance(transform.position, pos);
            var duration = distance / speed;
            
            transform.DOMove(pos, duration).SetEase(Ease.InQuad);
        }
    }
}
