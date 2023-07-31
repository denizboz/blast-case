using System;
using Events;
using Events.Implementations;

namespace Board.Cubes
{
    public abstract class Cube : Item
    {
        public override void AddToItemChain()
        {
            GameEventSystem.Invoke<CubeLinkedToChainEvent>(this);
        }
        
        public override void GetDestroyed()
        {
            base.GetDestroyed();
            GameEventSystem.Invoke<CubePoppedEvent>(this);
        }
        
        public override void OnTap()
        {
            GameEventSystem.Invoke<CubeTappedEvent>(this);
        }

        public override bool IsChainable(Type cubeType)
        {
            return cubeType == Type;
        }
    }
}
