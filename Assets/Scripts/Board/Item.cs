using DG.Tweening;
using UnityEngine;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer SRenderer;

        private const float fallSpeed = 6.5f;
        
        public void SetSprite(Sprite sprite)
        {
            SRenderer.sprite = sprite;
        }
        
        public void SetPositionAndSorting(int x, int y)
        {
            Position = new Vector2Int(x, y);
            SRenderer.sortingOrder = x;
        }

        public void FallTo(Vector3 pos)
        {
            var distance = Vector3.Distance(transform.position, pos);
            var duration = distance / fallSpeed;
            
            transform.DOMove(pos, duration).SetEase(Ease.InQuad);
        }
    }
}
