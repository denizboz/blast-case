using Events;
using Events.Implementations.BoardEvents;
using Managers;

namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item, ITappable
    {
        public CubeType Type;

        public const int VarietySize = 5;

        public void SetType(CubeType type)
        {
            Type = type;
        }

        public void OnTap()
        {
            BoardManager.DestroyNeighbouringItems(this);
        }

        public override void GetDestroyed()
        {
            ItemPooler.Return(this);
            BoardManager.RemoveItemFromBoard(this);
            GameEventSystem.Invoke<CubeDestroyedEvent>(this);
        }

        public override void AddToChain()
        {
            BoardManager.AddToChainedItems(this);
        }
    }
}
