/*
* Copyright (c) mmq
*/

using System.Collections.Generic;
using _Scripts.Cameras.FirstPerson;
using _Scripts.Cameras.FlyCam;
using _Scripts.InGameTime;
using _Scripts.ml_agents.Agents.Fox;
using _Scripts.ml_agents.Agents.Rabbit;
using _Scripts.WorldGeneration;
using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using _Scripts.WorldGeneration.Spawning;
using _Scripts.WorldGeneration.TerrainGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables

    // public
    public static GameManager Instance;

    // [SerializeField]
    [Header("General Settings")] 
    [Tooltip("Should be between 1 and ")]
    [SerializeField] private Vector2Int resolution = new(16, 16);

    [Range(25, 150)][SerializeField] private float maxTerrainHeight = 50.0f;

    [Tooltip("The height of the water level as a percentage of the maximum terrain height.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float waterLevel = 0.3f;

    [Tooltip("The height of the mountain area as a percentage of the maximum terrain height.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float mountainArea = 0.99f;

    [Space(10)] [Header("Seed")] [SerializeField]
    private bool useRandomSeed;

    [SerializeField] private string seed = "Labor Games 2023";

    [Space(10)] [Header("Terrain Generation")] 
    [SerializeField] public GeneralSettings generalSettings;
    [SerializeField] public GeneralSettings generalMenuSettings;

    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private WaterGenerator waterGenerator;

    [Space(10)] [Header("Asset Generation")]
    [Tooltip("A list of plants to generate.")] 
    [SerializeField] private List<PlantSettings> plantsList;
    [SerializeField] private List<PlantSettings> plantsListFromMenu;

    [Tooltip("A list of burrows to generate.")] 
    [SerializeField] private List<BurrowSettings> initialBurrowsList;
    [SerializeField] private List<BurrowSettings> initialBurrowsListFromMenu;

    [Tooltip("The burrow to generate.")] [SerializeField]
    private BurrowSettings burrow;

    [Space(10)] [Header("Watcher")] 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject firstPerson;
    [SerializeField] private FlyCam flyCam;

    private AssetManager _assetManager;
    
    private GeneralSettings _settings;

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

    private bool _terrainGenerated;

    // private
    private TimeManager _timer;
    private WorldManager _worldManager;
    private bool _initialWorldGenerated;
    private GameObject _fps;

    #endregion

    #region Unity Methods

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
        _assetManager = AssetManager.GetInstance();

        Checker.BurrowList = new List<GameObject>();

        Debug.Log("<color=#7dff33> Starting</color>");

        if (GenerateInitialWorld())
        {
            Checker.Running = true;
            _timer.BeginTimer();
        }

        Subscribe();
    }

    /// <summary>
    ///     Generate the initial world.
    /// </summary>
    /// <returns></returns>
    private bool GenerateInitialWorld()
    {
        // init
        Checker.Running = false;
        Checker.RabbitID = 0;
        Checker.FoxID = 0;

        _timer = TimeManager.Instance;

        if (Checker.IsStartingFromMenu)
        {
            _noiseWithClamp.NoiseGenerator = new NoiseGenerator(generalMenuSettings.seed);
            _noiseWithClamp.ValueClamp = new ValueClamp();
            
            _plants = new Plants
            {
                PlantsList = plantsListFromMenu,
                PlantParents = _plantParents
            };

            _burrows = new Burrows
            {
                BurrowsList = initialBurrowsListFromMenu,
                Burrow = burrow,
                BurrowParents = _burrowParents
            };
            
            _initialWorldGenerated = _worldManager.GenerateInitialWorld(generalMenuSettings.resolution, generalMenuSettings.maxTerrainHeight, waterLevel,
                mountainArea, generalMenuSettings, noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator,
                _assetManager, _plants, _burrows, firstPerson, flyCam, mainCamera);
        }
        else
        {            
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
            
            _initialWorldGenerated = _worldManager.GenerateInitialWorld(resolution, maxTerrainHeight, waterLevel,
                mountainArea, generalSettings, noiseSettings, _noiseWithClamp, groundGenerator, waterGenerator,
                _assetManager, _plants, _burrows, firstPerson, flyCam, mainCamera);
        }

        if (!_initialWorldGenerated) Debug.Log("Error while generating world");

        return _initialWorldGenerated;
    }

    /// <summary>
    ///     Subscribe to InGameTime events.
    /// </summary>
    private void Subscribe()
    {
        TimeManager.OnDayChanged += Check;
        TimeManager.OnMonthChanged += RespawnPlants;
    }

    /// <summary>
    ///     Reload the world.
    /// </summary>
    private void ReloadWorld()
    {
        Debug.Log("<color=#7dff33>Reload</color>");
        Checker.Running = false;

        // Delete possibly remaining rabbits
        if (Checker.RabbitCounter != 0)
            foreach (var rabbit in FindObjectsOfType<AgentRabbit>())
                if (rabbit)
                {
                    Checker.RabbitCounter--;
                    Destroy(rabbit.gameObject);
                }

        // Delete possibly remaining foxes
        if (Checker.FoxCounter != 0)
            foreach (var fox in FindObjectsOfType<AgentFox>())
                if (fox)
                {
                    Checker.FoxCounter--;
                    Destroy(fox.gameObject);
                }

        _timer.ResetTimer();

        Destroy(_worldManager.World);

        _worldManager = WorldManager.ResetInstance();
        _assetManager = AssetManager.ResetInstance();

        var scene = SceneManager.GetActiveScene();
        if (scene.name.Equals("WorldGen1"))
        {
            Debug.Log("<color=#7dff33>Reload: WorldGen2</color>");
            SceneManager.LoadScene("WorldGen2");
        }
        else if (scene.name.Equals("WorldGen2"))
        {
            Debug.Log("<color=#7dff33>Reload: WorldGen1</color>");
            SceneManager.LoadScene("WorldGen1");
        }
    }

    /// <summary>
    ///     Respawn plants.
    /// </summary>
    private void RespawnPlants()
    {
        _worldManager.RespawnPlants(_plants);
    }

    /// <summary>
    ///     Check if one of the counters is 0. Then reload the world.
    ///     Check if
    /// </summary>
    private void Check()
    {
        if (!Checker.Running) return;

        if (Checker.RabbitCounter <= 0 || Checker.FoxCounter <= 0) ReloadWorld();

        // Checker.CheckCarrots(_plants);
    }

    /// <summary>
    /// Change the current view from flyCam to FirstPerson and back.
    /// </summary>
    public void ChangeView()
    {
        switch (generalMenuSettings.watchMode)
        {
            case WatchMode.FlyCam:
            {
                generalMenuSettings.watchMode = WatchMode.FirstPerson;
                
                // Destroy FlyCam
                var pos = FindObjectOfType<FlyCam>().gameObject.transform.position;
                
                DestroyImmediate(FindObjectOfType<FlyCam>().gameObject);

                // Instantiate the first person controller
                _fps = Instantiate(firstPerson,
                    GeneratorFunctions.GetSurfacePointFromWorldCoordinate(pos.x, pos.z, resolution, maxTerrainHeight, _worldManager.Map,
                        generalMenuSettings), Quaternion.identity);
                pos = _fps.transform.position;
                pos.y += 2.5f;
                _fps.transform.position = pos;
                
                break;
            }
            case WatchMode.FirstPerson:
            {
                generalMenuSettings.watchMode = WatchMode.FlyCam;

                // Destroy
                var pos = FindObjectOfType<FirstPersonController>().gameObject.transform.position;
                
                DestroyImmediate(FindObjectOfType<FirstPersonController>().gameObject);
                
                // Instantiate the FlyCam
                Instantiate(flyCam.gameObject, new Vector3(pos.x, maxTerrainHeight + 25f, pos.z), Quaternion.identity);

                break;
            }
        }
    }

    #endregion
}