using Events;
using Events.Implementations.Board;
using Managers;
using UnityEngine;
using Utility;

namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item, ITappable
    {
        public CubeType Type;

        public const int VarietySize = 5;

        public void SetType(CubeType type)
        {
            Type = type;
        }

        public void OnTap()
        {
            BoardManager.DestroyNeighbouringItems(this);
        }

        public override void Setup(SpriteContainer container)
        {
            var cubeType = (CubeType)Random.Range(0, VarietySize);
            var sprite = container.GetSprite<Cube>(cubeType);
            SetSprite(sprite);
            SetType(cubeType);
        }

        public override void GetDestroyed()
        {
            ItemPooler.Return(this);
            BoardManager.RemoveItemFromBoard(this);
            GameEventSystem.Invoke<CubeDestroyedEvent>(this);
        }

        public override void AddToChain()
        {
            BoardManager.AddToChainedItems(this);
        }
    }
}
