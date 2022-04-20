using System;
using System.Collections.Generic;
using System.Linq;
using Lambda.Runtime.Generics;
using UnityEngine;

namespace Systems
{
    public class DungeonSpawnSystem
    {
        private readonly IList<GenericPool<GameObject>> _normalDungeonsPools;
        private readonly IList<GenericPool<GameObject>> _lightDungeonsPools;

        public DungeonSpawnSystem(IEnumerable<GameObject> normalDungeonsPrefabs, 
                                    IEnumerable<GameObject> lightDungeonsPrefabs, 
                                    Func<GameObject, GameObject> createMethod,
                                    Action<GameObject> resetMethod)
        {
            _normalDungeonsPools = new List<GenericPool<GameObject>>();
            _lightDungeonsPools = new List<GenericPool<GameObject>>();

            foreach (GameObject dungeonPrefab in normalDungeonsPrefabs)
                _normalDungeonsPools.Add(GenericPoolBuilder<GameObject>.Builder()
                                    .SetCreateMethod(() => createMethod(dungeonPrefab))
                                    .SetResetMethod(resetMethod)
                                    .SetFreeMethod(FreeDungeon)
                                    .Build());
            
            foreach (GameObject dungeonPrefab in lightDungeonsPrefabs)
                _lightDungeonsPools.Add(GenericPoolBuilder<GameObject>.Builder()
                                    .SetCreateMethod(() => createMethod(dungeonPrefab))
                                    .SetResetMethod(resetMethod)
                                    .SetFreeMethod(FreeDungeon)
                                    .Build());
        }

        public GameObject GetNormalDungeon()
        {
            return GetDungeonFromPools(_normalDungeonsPools);
        }
        
        public GameObject GetLightDungeon()
        {
            return GetDungeonFromPools(_lightDungeonsPools);
        }
        
        public void ReleaseDungeon(GameObject dungeon)
        {
            bool wasReleased = ReleaseDungeon(dungeon, _normalDungeonsPools) ||
                               ReleaseDungeon(dungeon, _lightDungeonsPools);

            if (!wasReleased)
            {
                Debug.Log($"The dungeon '{dungeon.name}' was not found on any pool. It was destroyed.");
                UnityEngine.Object.Destroy(dungeon);
            }
        }

        private static GameObject GetDungeonFromPools(IList<GenericPool<GameObject>> pools)
        {
            if (pools.Count == 0)
                throw new IndexOutOfRangeException("Failed to get dungeon. No dungeons pools were previously created");
            return pools[UnityEngine.Random.Range(0, pools.Count)].Get();
        }
        
        private static bool ReleaseDungeon(GameObject dungeon, IList<GenericPool<GameObject>> pools)
        {
            var pool = pools.FirstOrDefault(x => x.Contains(dungeon));
            if (pool == null)
                return false;
            
            pool.Release(dungeon);
            return true;
        }
        
        private static void FreeDungeon(GameObject dungeon)
        {
            UnityEngine.Object.Destroy(dungeon);
        }
    }
}
