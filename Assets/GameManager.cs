using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGameTime;
using ml_agents.Agents.Handler;
using UnityEngine;
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

    [SerializeField] private GeneralSettings generalSettings;
    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private WaterGenerator waterGenerator;

    [Tooltip("A list of food to generate.")] [SerializeField]
    private List<PlantSettings> foodList;

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
    private WorldManager _worldManager;
    [SerializeField] private AssetManager _assetManager;

    private NoiseWithClamp _noiseWithClamp;

    private float[,] _map;

    private Food _food;
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
        _timer = TimeManager.Instance;
        _timer.BeginTimer();

        _worldManager = WorldManager.GetInstance();
        _assetManager = AssetManager.GetInstance();

        _noiseWithClamp.NoiseGenerator = new NoiseGenerator(noiseSettings);
        _noiseWithClamp.ValueClamp = new ValueClamp();

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

        if (Input.GetKeyDown(KeyCode.Alpha1) && _running) _timer.SetTimeScale(1.0f);

        if (Input.GetKeyDown(KeyCode.Alpha2) && _running) _timer.SetTimeScale(2.0f);

        if (Input.GetKeyDown(KeyCode.Alpha3) && _running) _timer.SetTimeScale(3.0f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadWorld();
        }

        // TimeManager.OnMonthChanged += RespawnPlants;
    }

    private void GenerateInitialWorld()
    {
        _food.FoodList = foodList;
        _food.FoodParents = _foodParents;

        _plants.PlantsList = plantsList;
        _plants.PlantParents = _plantParents;

        _burrows.BurrowsList = initialBurrowsList;
        _burrows.Burrow = burrow;
        _burrows.BurrowParents = _burrowParents;

        _running = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
            noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator, _assetManager, _food, _plants, _burrows,
            out _map);
    }

    private void ReloadWorld()
    {
        if (_running)
        {
            _plants.PlantsList = plantsList;
            _plants.PlantParents = _plantParents;

            _burrows.BurrowsList = initialBurrowsList;
            _burrows.Burrow = burrow;
            _burrows.BurrowParents = _burrowParents;

            _running = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator, _assetManager, _food, _plants,
                _burrows, out _map);
        }
    }

    private void RespawnPlants()
    {
        _assetManager.SpawnPlants();
    }
}