using Events;
using Events.Implementations;
using UnityEngine;
using Utilities;

namespace Board
{
    public class Duck : Item
    {
        public override void Setup(SpriteContainer container)
        {
            var sprite = container.GetSprite<Duck>();
            SetSprite(sprite);
        }

        protected override void OnFallComplete(bool hitBottom)
        {
            if (!hitBottom)
                return;
            
            GameEventSystem.Invoke<DuckHitBottomEvent>(this);
        }
    }
}
