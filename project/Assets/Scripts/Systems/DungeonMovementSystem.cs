using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class DungeonMovementSystem : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        [Range(-1,1)]
        private int direction = -1;
        [SerializeField]
        private float dungeonLimitDistance = 25f;
    
        private IList<Transform> _childs = new List<Transform>();    
        private bool _isGameRunning = false;

        private void Awake()
        {
            // Get spawn 
            foreach (Transform child in transform)
                _childs.Add(child);
            
            GameEvents.OnDungeonCreated += AddDungeon;
            GameEvents.OnGameStarts += OnGameStarts;
            GameEvents.OnPlayerDies += OnPlayerDies;
        }

        private void OnGameStarts()
        {
            _isGameRunning = true;
        }

        private void OnPlayerDies()
        {
            _isGameRunning = false;
        }

        private void AddDungeon(GameObject dungeon)
        {
            _childs.Add(dungeon.transform);
        }
        
        private void DungeonLimitReached(Transform child)
        {
            _childs.Remove(child);
            GameEvents.DispatchOnDungeonLimitReached(child.gameObject);
        }
        
        private void Update()
        {
            if(!_isGameRunning) return;
            
            for (int i = _childs.Count - 1; i >= 0; i--)
            {
                if (_childs[i].localPosition.z < dungeonLimitDistance)
                    _childs[i].localPosition += transform.forward * (direction * speed * GameManager.Instance.CurrentGameSpeed * Time.deltaTime);
                else
                    DungeonLimitReached(_childs[i]);
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnDungeonCreated -= AddDungeon;
            GameEvents.OnGameStarts -= OnGameStarts;
            GameEvents.OnPlayerDies -= OnPlayerDies;
        }
    }
}
