using Events;
using Events.Implementations.BoardEvents;
using Managers;

namespace Board
{
    public class Balloon : Item
    {
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
