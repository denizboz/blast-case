using System;
using System.Threading.Tasks;
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

        private Vector3 fallPos;
        private float fallDuration;
        
        private const float fallSpeed = 12.5f;

        
        public virtual void OnTap() { }
        public virtual void OrientRandom() { }
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
        
        public async void MoveTo(Vector3 pos, float delay = 0f)
        {
            var distance = Vector3.Distance(transform.position, pos);

            fallPos = pos;
            fallDuration = distance / fallSpeed;
            
            await Task.Delay(TimeSpan.FromSeconds(delay));
            transform.DOMove(fallPos, fallDuration).SetEase(Ease.InQuad).OnComplete(OnFallComplete);
        }

        protected virtual void OnFallComplete() { }
    
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
