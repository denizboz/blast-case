using Events;
using Events.Implementations.BoardEvents;
using Managers;

namespace Board
{
    public class Duck : Item
    {
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
