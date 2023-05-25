using System.Collections.Generic;
using System.ComponentModel.Design;
using InGameTime;
using ml_agents.Agents.fox;
using ml_agents.Agents.rabbit;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldGeneration._Scripts;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;

public class GameManager : MonoBehaviour
{
    // [SerializeField]
    [Header("General Settings")] [SerializeField]
    private Vector2Int resolution = new(128, 128);

    [SerializeField] private float maxTerrainHeight = 180.0f;

    [Tooltip("The height of the water level as a percentage of the maximum terrain height.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float waterLevel = 0.3f;

    [Header("Seed")] [SerializeField] private bool useRandomSeed;
    [SerializeField] private string seed = "Hello World!";

    [SerializeField] private GeneralSettings generalSettings;
    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private WaterGenerator waterGenerator;

    [SerializeField] private AssetManager assetManager;

    [Tooltip("A list of plants to generate.")] [SerializeField]
    private List<PlantSettings> plantsList;

    [Tooltip("A list of burrows to generate.")] [SerializeField]
    private List<BurrowSettings> initialBurrowsList;

    [Tooltip("The burrow to generate.")] [SerializeField]
    private BurrowSettings burrow;

    // public
    public static GameManager Instance;
    private Dictionary<BurrowSettings, Transform> _burrowParents;
    private Burrows _burrows;
    private bool _burrowsGenerated;
    private Dictionary<PlantSettings, Transform> _foodParents;

    private float[,] _map;

    private NoiseWithClamp _noiseWithClamp;

    private Transform _parent;
    private Dictionary<PlantSettings, Transform> _plantParents;

    private Plants _plants;
    private bool _plantsGenerated;

    private static bool _running = false;
    private bool _reloading = true;

    private bool _terrainGenerated;

    // private
    private TimeManager _timer;
    private WorldManager _worldManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _worldManager = WorldManager.GetInstance();
        assetManager = AssetManager.GetInstance();
        
        Debug.Log("Starting");

        GenerateInitialWorld();
    }

    // Update is called once per frame
    private void Update()
    {
        // Only for Debugging
        // if (Input.GetKeyDown(KeyCode.P) && _running)
        // {
        //     _timer.Stop();
        //     _running = false;
        // }
        // else if (Input.GetKeyDown(KeyCode.P) && !_running)
        // {
        //     _timer.Resume();
        //     _running = true;
        // }
        //
        // if (Input.GetKeyDown(KeyCode.K) && _running)
        // {
        //     var rabbits = _worldManager.rabbitList;
        //     for (int i = 0; i < rabbits.Count; i++)
        //     {
        //         rabbits[i].TryGetComponent<CustomAgent>(out var agent);
        //         _worldManager.rabbitList.Remove(rabbits[i]);
        //         agent.DestroyAgent();
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.Alpha1) && _running) _timer.SetTimeScale(1.0f);

        if (Input.GetKeyDown(KeyCode.Alpha2) && _running) _timer.SetTimeScale(2.0f);

        if (Input.GetKeyDown(KeyCode.Alpha3) && _running) _timer.SetTimeScale(3.0f);

        if (Input.GetKeyDown(KeyCode.R) && /*_running*/ Checker.running)
        {
            _running = false;
            ReloadWorld();
        }

        TimeManager.OnMonthChanged += RespawnPlants;

        if (Checker.running)
        {
            Check();
        }
    }

    private void GenerateInitialWorld()
    {
        Checker.running = false;
        _running = false;
        
        _worldManager.burrowList = new List<GameObject>();
        _worldManager.rabbitList = new List<GameObject>();
        _worldManager.foxList = new List<GameObject>();

        _timer = TimeManager.Instance;

        // Check if a random seed is wanted
        if (useRandomSeed) seed = Time.realtimeSinceStartupAsDouble.ToString();

        _noiseWithClamp.NoiseGenerator = new NoiseGenerator(seed);
        _noiseWithClamp.ValueClamp = new ValueClamp();

        _plants = new Plants
        {
            PlantsList = plantsList,
            PlantParents = _plantParents
        };

        _burrows = new Burrows
        {
            BurrowsList = initialBurrowsList,
            Burrow = burrow,
            BurrowParents = _burrowParents
        };

        Checker.running = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
            noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator, assetManager, _plants, _burrows);

        _running = Checker.running;
        
        if (Checker.running)
            _timer.BeginTimer();
        else
            Debug.Log("Error while generating world");
    }

    private void ReloadWorld()
    {
        Checker.running = false;
        _running = false;
        
        foreach (var rabbit in _worldManager.rabbitList)
        {
            rabbit.TryGetComponent<AgentRabbit>(out var agent);
            agent.DestroyAgent();
        }

        foreach (var rabbit in _worldManager.foxList)
        {
            rabbit.TryGetComponent<AgentFox>(out var agent);
            agent.DestroyAgent();
        }

        _timer.ResetTimer();

        Destroy(_worldManager.world);

        _worldManager = WorldManager.ResetInstance();
        assetManager = AssetManager.ResetInstance();

        // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // // Check if a random seed is wanted
        // if (useRandomSeed)
        // {
        //     seed = Time.realtimeSinceStartupAsDouble.ToString();
        // }
        //
        // _noiseWithClamp.NoiseGenerator = new NoiseGenerator(noiseSettings, seed);
        // _noiseWithClamp.ValueClamp = new ValueClamp();
        //
        // _plants = new Plants
        // {
        //     PlantsList = plantsList,
        //     PlantParents = _plantParents
        // };
        //
        // _burrows = new Burrows
        // {
        //     BurrowsList = initialBurrowsList,
        //     Burrow = burrow,
        //     BurrowParents = _burrowParents
        // };
        //
        // _running = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
        //     noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator, assetManager, _plants, _burrows);
        //
        // if (_running)
        // {
        //     _timer.BeginTimer();
        // }
        // else
        // {
        //     Debug.Log("Error while generating world");
        // }
    }

    private void RespawnPlants()
    {
        assetManager.SpawnPlants();
    }

    void Check()
    {
        if ((_worldManager.rabbitList.Count == 0 || _worldManager.foxList.Count == 0) && Checker.running == true && _running)
        {
            Debug.Log("Reload cause 0");
            ReloadWorld();
        }
    }
}