using System;
using Board;

namespace Managers
{
    public enum CoreEvent { BoardLoaded, GameWon, GameLost }
    public enum BoardEvent { ItemTapped }
    
    public static class GameEvents
    {
        private static event Action boardLoaded, gameWon, gameLost;
        private static event Action<Item> cubeHitEvent, rocketHitEvent;
        
        
        private static readonly Action[] coreEvents = new Action[]
        {
            boardLoaded, gameWon, gameLost
        };

        private static readonly Action<Item>[] boardEvents = new Action<Item>[]
        {
            cubeHitEvent, rocketHitEvent
        };

        
        public static void Invoke(CoreEvent coreEvent)
        {
            coreEvents[(int)coreEvent]?.Invoke();
        }
        
        public static void Invoke(BoardEvent boardEvent, Item item)
        {
            boardEvents[(int)boardEvent]?.Invoke(item);
        }

        public static void AddListener(CoreEvent coreEvent, Action action)
        {
            coreEvents[(int)coreEvent] += action;
        }
        
        public static void AddListener(BoardEvent boardEvent, Action<Item> action)
        {
            boardEvents[(int)boardEvent] += action;
        }
        
        public static void RemoveListener(CoreEvent coreEvent, Action action)
        {
            coreEvents[(int)coreEvent] -= action;
        }
        
        public static void RemoveListener(BoardEvent boardEvent, Action<Item> action)
        {
            boardEvents[(int)boardEvent] -= action;
        }
    }
}
