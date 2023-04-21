using System;
using Board;

namespace Managers
{
    public enum CoreEvent { BoardLoaded, MoveMade, GameWon, GameLost }
    public enum BoardEvent { ItemTapped, CubeDestroyed, BalloonPopped, DuckHitBottom, BalloonDestroyed, DuckDestroyed }
    
    public static class GameEvents
    {
        private static event Action boardLoaded, moveMade, gameWon, gameLost;
        private static event Action<Item> itemTapped, cubeDestroyed, balloonPopped, duckHitBottom, balloonDestroyed, duckDestroyed;
        
        
        private static readonly Action[] coreEvents = new Action[]
        {
            boardLoaded, moveMade, gameWon, gameLost
        };

        private static readonly Action<Item>[] boardEvents = new Action<Item>[]
        {
            itemTapped, cubeDestroyed, balloonPopped, duckHitBottom, balloonDestroyed, duckDestroyed
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
