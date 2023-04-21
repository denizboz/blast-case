using UnityEngine;
using DG.Tweening;
using Managers;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer SRenderer;

        private const float fallSpeed = 7f;
        
        public void SetSprite(Sprite sprite)
        {
            if (this is Booster)
                return;
            
            SRenderer.sprite = sprite;
        }
        
        public void SetGridPositionAndSorting(Vector2Int gridPosition)
        {
            Position = gridPosition;
            
            if (this is Booster)
                return;
            
            SRenderer.sortingOrder = gridPosition.x;
        }

        public void SetWorldPosition(Vector3 worldPos)
        {
            transform.position = worldPos;
        }
        
        public void MoveTo(Vector3 pos, float speed = fallSpeed, bool isDuck = false)
        {
            var distance = Vector3.Distance(transform.position, pos);
            var duration = distance / speed;

            if (!isDuck)
                transform.DOMove(pos, duration).SetEase(Ease.InQuad);
            else
                transform.DOMove(pos, duration).SetEase(Ease.InQuad).OnComplete(() => GameEvents.Invoke(BoardEvent.DuckHitBottom, this));
        }
    }
}
