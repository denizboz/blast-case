using Events;
using Events.Implementations;

namespace Board
{
    public class Duck : Item
    {
        protected override void OnFallComplete(bool hitBottom)
        {
            if (!hitBottom)
                return;
            
            GameEventSystem.Invoke<DuckHitBottomEvent>(this);
        }
    }
}
