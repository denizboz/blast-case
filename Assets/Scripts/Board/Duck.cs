using Events;
using Events.Implementations;
using Managers;

namespace Board
{
    public class Duck : Item
    {
        protected override void OnFallComplete()
        {
            if (Position.x == BoardManager.Bottom)
                GameEventSystem.Invoke<DuckHitBottomEvent>(this);
        }
    }
}
