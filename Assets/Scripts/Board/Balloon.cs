using Events;
using Events.Implementations.Board;
using Managers;
using Utility;

namespace Board
{
    public class Balloon : Item
    {
        public override void Setup(SpriteContainer container)
        {
            var sprite = container.GetSprite<Balloon>();
            SetSprite(sprite);
        }

        public override void GetDestroyed()
        {
            ItemPooler.Return(this);
            BoardManager.RemoveItemFromBoard(this);
            GameEventSystem.Invoke<BalloonDestroyedEvent>(this);
        }

        public override void AddToChain()
        {
            BoardManager.AddToChainedItems(this);
        }
    }
}
