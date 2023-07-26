using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Managers;
using UnityEngine;
using Utilities;

namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item
    {
        public CubeType Type;

        public const int VarietySize = 5;

        public override void AddToItemChain()
        {
            GameEventSystem.Invoke<CubeLinkedToChainEvent>(this);
        }

        public override void OnTap()
        {
            GameEventSystem.Invoke<CubeTappedEvent>(this);
        }

        public override void GetDestroyed()
        {
            base.GetDestroyed();
            GameEventSystem.Invoke<CubePoppedEvent>();
        }

        public override bool IsChainable(CubeType type)
        {
            return type == Type;
        }

        private void SetType(CubeType type)
        {
            Type = type;
        }
        
        public override void Setup(SpriteContainer container)
        {
            var cubeType = (CubeType)Random.Range(0, VarietySize);
            var sprite = container.GetSprite<Cube>(cubeType);
            SetSprite(sprite);
            SetType(cubeType);
        }
    }
}
