using System;
using UnityEngine;
using DG.Tweening;
using Events;
using Events.Implementations;

namespace Board
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2Int Position;
        public SpriteRenderer Renderer;
        
        public Type Type => GetType();
        public Sprite Sprite => Renderer.sprite;
        public Vector3 WorldPosition => transform.position;

        private const float fallSpeed = 9f;

        
        public virtual void OnTap() { }

        public virtual bool IsChainable(Type cubeType)
        {
            return false;
        }
        
        public virtual void AddToItemChain() { }
        
        public virtual void GetDestroyed()
        {
            GameEventSystem.Invoke<ItemDestroyedEvent>(this);
        }
        
        public void SetGridPositionAndSorting(Vector2Int gridPosition)
        {
            Position = gridPosition;
            
            if (this is Booster)
                return;
            
            Renderer.sortingOrder = gridPosition.x;
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            var hasRenderer = TryGetComponent(out SpriteRenderer sr);

            if (hasRenderer)
                Renderer = sr;
        }
#endif
    }
    
    [Serializable]
    public struct ItemProperty<T, U> where T : Item
    {
        public T Item;
        public U Property;
    }
}
