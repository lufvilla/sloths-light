using System;
using UnityEngine;

namespace Systems
{
    // This should be an event system...
    public static class GameEvents
    {
        public static event Action OnGameStarts = delegate {};
        public static void DispatchOnGameStarts()
        {
            OnGameStarts?.Invoke();
        }
        
        public static event Action OnPlayerDies = delegate {};
        public static void DispatchOnPlayerDies()
        {
            OnPlayerDies?.Invoke();
        }
        
        public static event Action<GameObject> OnDungeonCreated = delegate {};
        public static void DispatchOnDungeonCreated(GameObject dungeon)
        {
            OnDungeonCreated?.Invoke(dungeon);
        }
        
        public static event Action<GameObject> OnDungeonLimitReached = delegate {};
        public static void DispatchOnDungeonLimitReached(GameObject dungeon)
        {
            OnDungeonLimitReached?.Invoke(dungeon);
        }
    }
}