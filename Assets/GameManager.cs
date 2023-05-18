using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGameTime;
using ml_agents.Agents;
using ml_agents.Agents.Handler;
using ml_agents.Agents.rabbit;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    public GameManager instance;

    // private
    private TimeManager _timer;
    [SerializeField] private WorldManager _worldManager;

    private NoiseWithClamp _noiseWithClamp;

    private float[,] map;

    private Plants _plants;
    private Burrows _burrows;

    private bool _terrainGenerated;
    private bool _plantsGenerated;
    private bool _burrowsGenerated;

    private Transform _parent;
    private Dictionary<PlantSettings, Transform> _foodParents;
    private Dictionary<PlantSettings, Transform> _plantParents;
    private Dictionary<BurrowSettings, Transform> _burrowParents;

    private bool _running;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _worldManager = WorldManager.GetInstance();
        assetManager = AssetManager.GetInstance();

        GenerateInitialWorld();
    }

    // Update is called once per frame
    void Update()
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

        if (Input.GetKeyDown(KeyCode.R) && _running) ReloadWorld();

        // TimeManager.OnDayChanged += print;

        TimeManager.OnMonthChanged += RespawnPlants;
            
        if (_worldManager.rabbitList.Count == 0) ReloadWorld();
    }

    private void print()
    {
        Debug.Log(_timer.GetCurrentDate().PrintToString("/"));
    }

    private void GenerateInitialWorld()
    {
        _timer = TimeManager.Instance;

        // Check if a random seed is wanted
        if (useRandomSeed)
        {
            seed = Time.realtimeSinceStartupAsDouble.ToString();
        }

        _noiseWithClamp.NoiseGenerator = new NoiseGenerator(noiseSettings, seed);
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

        _running = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
            noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator, assetManager, _plants, _burrows);

        if (_running)
        {
            _timer.BeginTimer();
        }
        else
        {
            Debug.Log("Error while generating world");
        }
    }

    private void ReloadWorld()
    {
        Debug.Log("Reload World");
        foreach (var rabbit in _worldManager.rabbitList)
        {
            rabbit.TryGetComponent<AgentRabbit>(out var agent);
            agent.DestroyAgent();
        }

        _timer.ResetTimer();

        Destroy(_worldManager.World);

        // _worldManager = new WorldManager();
        // assetManager = new AssetManager();

        _worldManager = WorldManager.ResetInstance();
        assetManager = AssetManager.ResetInstance();

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

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
}