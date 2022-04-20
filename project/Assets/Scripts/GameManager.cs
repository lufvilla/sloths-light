using System.Globalization;
using Systems;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;
    
    public float CurrentGameSpeed { get; private set; } = 1f;
    public bool IsGameRunning { get; private set; } = false;

    [Header("Dungeon Generation")]
    public GameObject[] lightDungeons;
    public GameObject[] normalDungeons;

    // Tendria que ser automatico U.U
    public float creationRate = 0.5f;

    [SerializeField]
    private float gameDifficultyIncrease = 0.005f;
    
    [SerializeField]
    private Transform startPositionTransform;
    private Vector3 _startPosition = Vector3.zero;

    [Header("Death Settings")]
    public GameObject deadScreen;
    public Text deadScreenText;
    public float timeToRestart = 5f;
    public int lightsSpace = 3;

    // Counters
    private float _currentTimeToCreate;
    private float _currentTimeToRestart;
    private float _noLightCount = 3;

    private bool _isPlayerDead = false;

    private DungeonSpawnSystem _spawnSystem;

    private void Awake()
    {
        Instance = this;
        _noLightCount = lightsSpace;
        _startPosition = startPositionTransform.position;
        _currentTimeToRestart = timeToRestart;

        _spawnSystem = new DungeonSpawnSystem(normalDungeons, lightDungeons, CreateDungeonMethod, ResetDungeonMethod);
        
        GameEvents.OnDungeonLimitReached += OnDungeonLimitReached;
        GameEvents.OnPlayerDies += OnPlayerDies;
    }

    private void Start()
    {
        GameEvents.DispatchOnGameStarts();
        IsGameRunning = true;
    }

    private void Update()
    {
        if (IsGameRunning)
            SpawnDungeonTimer();
        else if (_isPlayerDead)
            DeadScreenTimer();
    }

    private void OnPlayerDies()
    {
        _isPlayerDead = true;
        IsGameRunning = false;
        if (deadScreen) deadScreen.SetActive(true);
    }
    
    private GameObject CreateDungeonMethod(GameObject dungeonPrefab)
    {
        return Instantiate(dungeonPrefab, _startPosition, Quaternion.identity, transform);
    }

    private void ResetDungeonMethod(GameObject dungeon)
    {
        dungeon.SetActive(false);
        dungeon.transform.position = _startPosition;
        dungeon.transform.rotation *= Quaternion.Euler(0,180f,0);
    }
    
    private void OnDungeonLimitReached(GameObject dungeon)
    {
        _spawnSystem.ReleaseDungeon(dungeon);
    }

    private void DeadScreenTimer()
    {
        if (deadScreenText)
            deadScreenText.text = Mathf.Round(_currentTimeToRestart).ToString(CultureInfo.InvariantCulture);

        _currentTimeToRestart -= Time.deltaTime;

        if (_currentTimeToRestart < 0)
            SceneLoaderSystem.Load(SceneID.Game);
    }


    private void SpawnDungeonTimer()
    {
        if (_currentTimeToCreate <= 0)
        {
            _noLightCount--;
            if (_noLightCount <= 0)
            {
                SpawnDungeon(true);
                _noLightCount = 3;
            }
            else
                SpawnDungeon();

            _currentTimeToCreate = creationRate;
            CurrentGameSpeed = Mathf.Clamp(CurrentGameSpeed + gameDifficultyIncrease, 1f, 3f);
        }

        _currentTimeToCreate -= Time.deltaTime * CurrentGameSpeed;
    }

    private void SpawnDungeon(bool isLight = false)
    {
        GameObject dungeon = (isLight) ? _spawnSystem.GetLightDungeon() : _spawnSystem.GetNormalDungeon();
        dungeon.SetActive(true);

        GameEvents.DispatchOnDungeonCreated(dungeon);
    }

    private void OnDestroy()
    {
        GameEvents.OnDungeonLimitReached -= OnDungeonLimitReached;
        GameEvents.OnPlayerDies -= OnPlayerDies;
    }
}
