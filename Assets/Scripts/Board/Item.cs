using UnityEngine;
using DG.Tweening;
using Events;
using Events.Implementations;
using Utilities;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer SRenderer;

        public Vector3 WorldPosition => transform.position;

        private const float fallSpeed = 9f;

        public abstract void Setup(SpriteContainer container);

        public virtual void OnTap() { }

        public virtual bool IsChainable(CubeType cubeType)
        {
            return false;
        }
        
        public virtual void AddToItemChain() { }
        
        public virtual void GetDestroyed()
        {
            GameEventSystem.Invoke<ItemDestroyedEvent>(this);
        }
        
        public void SetSprite(Sprite sprite)
        {
            if (SRenderer == null)
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
        
        public void MoveTo(Vector3 pos, bool toBottom)
        {
            var distance = Vector3.Distance(transform.position, pos);
            var duration = distance / fallSpeed;
            
            transform.DOMove(pos, duration).SetEase(Ease.InQuad).OnComplete(() => OnFallComplete(toBottom));
        }
        
        protected virtual void OnFallComplete(bool hitBottom) { }
    }
}
