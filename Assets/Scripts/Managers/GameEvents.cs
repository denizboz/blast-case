using System;
using Board;

namespace Managers
{
    public enum CoreEvent { MetaLoaded, BoardLoaded, GameWon, GameLost }
    public enum BoardEvent { CubeHit, RocketHit, DuckHit }
    
    public static class GameEvents
    {
        private static event Action metaLoadedEvent, boardLoadedEvent, gameWonEvent, gameLostEvent;
        
        private static event Action<Item> cubeHitEvent, rocketHitEvent, duckHitEvent;

        private static readonly Action[] coreEvents = new Action[]
        {
            metaLoadedEvent, boardLoadedEvent, gameWonEvent, gameLostEvent
        };

        private static readonly Action<Item>[] boardEvents = new Action<Item>[]
        {
            cubeHitEvent, rocketHitEvent, duckHitEvent
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
