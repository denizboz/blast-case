using System;
using Events;
using Events.Implementations;

namespace Board
{
    public class Balloon : Item
    {
        public override bool IsChainable(Type type)
        {
            return true;
        }
        
        public override void GetDestroyed()
        {
            base.GetDestroyed();
            GameEventSystem.Invoke<BalloonPoppedEvent>(this);
        }
        
        public override void AddToItemChain()
        {
            GameEventSystem.Invoke<BalloonAddedToChainEvent>(this);
        }
    }
}
