using Events;
using Events.Implementations.Board;
using Managers;
using Utility;

namespace Board
{
    public class Duck : Item
    {
        public override void Setup(SpriteContainer container)
        {
            var sprite = container.GetSprite<Duck>();
            SetSprite(sprite);
        }

        public override void GetDestroyed()
        {
            ItemPooler.Return(this);
            GameEventSystem.Invoke<DuckDestroyedEvent>(this);
        }

        public override void AddToChain()
        {
        }
    }
}
