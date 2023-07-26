using Events;
using Events.Implementations;
using Managers;
using Utilities;

namespace Board
{
    public class Balloon : Item
    {
        public override void Setup(SpriteContainer container)
        {
            var sprite = container.GetSprite<Balloon>();
            SetSprite(sprite);
        }

        public override bool IsChainable(CubeType none)
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
