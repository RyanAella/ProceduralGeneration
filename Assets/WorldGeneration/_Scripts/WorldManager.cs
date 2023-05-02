using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;
using WorldGeneration._Scripts.Spawning.TerrainAssets;

namespace WorldGeneration._Scripts
{
    public class WorldManager : MonoBehaviour
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
        [SerializeField] private GameObject wallPrefab;

        [Tooltip("A list of plants to generate.")] [SerializeField]
        private List<AssetSettings> plants;

        [Tooltip("A list of burrows to generate.")] [SerializeField]
        private List<BurrowSettings> burrows;

        // private
        public WorldManager instance;
        private AssetManager _assetManager;
        private bool plantsGenerated;
        private bool _burrowsGenerated;

        private Vector2[] _corners;
        private float[,] _map;
        private NoiseWithClamp _noiseWithClamp;
        private Transform _parent;
        private Dictionary<AssetSettings, Transform> _plantParents;
        private Dictionary<BurrowSettings, Transform> _burrowParents;

        private bool _running;
        private bool _terrainGenerated;
        private TimeManager _timer;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this) Destroy(gameObject);
        }

        // Start is called before the first frame update
        private void Start()
        {
            _timer = TimeManager.Instance;
            _timer.BeginTimer();
            _assetManager = AssetManager.GetInstance();

            _noiseWithClamp.NoiseGenerator = new NoiseGenerator(noiseSettings);
            _noiseWithClamp.ValueClamp = new ValueClamp();

            _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                noiseSettings, wallPrefab, _noiseWithClamp, out _map);

            if (plants.Count != 0 && _terrainGenerated)
            {
                _plantParents = new Dictionary<AssetSettings, Transform>();
                foreach (var assetSetting in plants)
                {
                    _plantParents.Add(assetSetting, Instantiate(assetSetting.parent, transform));
                    assetSetting.assets = new List<GameObject>();
                    _assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel, generalSettings, _map,
                        assetSetting, _plantParents[assetSetting]);
                }

                plantsGenerated = true;
            }

            if (burrows.Count != 0 && _terrainGenerated)
            {
                _burrowParents = new Dictionary<BurrowSettings, Transform>();

                var burrowSetting = burrows[0];
                _burrowParents.Add(burrowSetting, Instantiate(burrowSetting.parent, transform));
                burrowSetting.assets = new List<GameObject>();
                _assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel, generalSettings, _map,
                    burrowSetting, _burrowParents[burrowSetting]);


                _burrowsGenerated = true;
            }

            _running = true;
        }

        // Update is called once per frame
        private void Update()
        {
            // Only for Debugging
            if (Input.GetKeyDown(KeyCode.P) && _running)
            {
                _timer.Stop();
                _running = false;
            }
            else if (Input.GetKeyDown(KeyCode.P) && !_running)
            {
                _timer.Resume();
                _running = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && _running) _timer.SetTimeScale(1.0f);

            if (Input.GetKeyDown(KeyCode.Alpha2) && _running) _timer.SetTimeScale(2.0f);

            if (Input.GetKeyDown(KeyCode.Alpha3) && _running) _timer.SetTimeScale(3.0f);

            // Reload Terrain
            if (Input.GetKey(KeyCode.R) && _running)
            {
                _assetManager = AssetManager.GetInstance();

                _noiseWithClamp.NoiseGenerator = new NoiseGenerator(noiseSettings);
                _noiseWithClamp.ValueClamp = new ValueClamp();

                _terrainGenerated = GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings,
                    noiseSettings, wallPrefab, _noiseWithClamp, out _map);

                if (plants.Count != 0 && _terrainGenerated)
                {
                    if (plantsGenerated)
                    {
                        foreach (var parent in _plantParents) Destroy(parent.Value.gameObject);
                    }

                    _plantParents = new Dictionary<AssetSettings, Transform>();
                    foreach (var assetSetting in plants)
                    {
                        // Destroy(_parents[assetSetting]);
                        _parent = Instantiate(assetSetting.parent, transform);
                        _plantParents.Add(assetSetting, _parent);
                        assetSetting.assets = new List<GameObject>();
                        _assetManager.InitialSpawnPlants(resolution, maxTerrainHeight, waterLevel, generalSettings,
                            _map, assetSetting, _parent);
                    }

                    plantsGenerated = true;
                }

                if (burrows.Count != 0 && _terrainGenerated)
                {
                    if (_burrowsGenerated)
                    {
                        foreach (var parent in _burrowParents)
                        {
                            // for (int i = 0; i < parent.Key.assetPrefab.GetComponent<RabbitBurrow>().inhabitants.Count; i++)
                            // {
                            //     var rabbit = parent.Key.assetPrefab.GetComponent<RabbitBurrow>().inhabitants[i];
                            //     DestroyImmediate(rabbit, true);
                            // }

                            if (parent.Key.assetPrefab.GetComponent<RabbitBurrow>().inhabitants.Count == 0)
                            {
                                Destroy(parent.Value.gameObject);
                            }
                        }
                    }


                    _burrowParents = new Dictionary<BurrowSettings, Transform>();

                    var burrowSetting = burrows[0];
                    _parent = Instantiate(burrowSetting.parent, transform);
                    _burrowParents.Add(burrowSetting, _parent);
                    burrowSetting.assets = new List<GameObject>();
                    _assetManager.InitialSpawnBurrows(resolution, maxTerrainHeight, waterLevel, generalSettings, _map,
                        burrowSetting, _burrowParents[burrowSetting]);

                    _burrowsGenerated = true;
                }
            }

            if (_terrainGenerated && plantsGenerated)
                foreach (var assetSetting in plants)
                    if (assetSetting.assets.Count < assetSetting.maxNumber)
                    {
                        var count = assetSetting.maxNumber - assetSetting.assets.Count;
                        // _parent = _parents[assetSetting];
                        _assetManager.SpawnPlants(resolution, maxTerrainHeight, waterLevel, generalSettings, _map,
                            assetSetting,
                            _plantParents[assetSetting], count);
                    }
        }

        /// <summary>
        /// </summary>
        /// <param name="maxTerrainHeight"></param>
        /// <param name="waterLevel"></param>
        /// <param name="generalSettings"></param>
        /// <param name="noiseSettings"></param>
        /// <param name="wallPrefab"></param>
        /// <param name="resolution"></param>
        /// <param name="noiseWithClamp"></param>
        /// <param name="map"></param>
        private bool GenerateTerrain(Vector2Int resolution, float maxTerrainHeight, float waterLevel,
            GeneralSettings generalSettings, NoiseSettings noiseSettings, GameObject wallPrefab,
            NoiseWithClamp noiseWithClamp, out float[,] map)
        {
            Instantiate(groundGenerator, transform);
            var _groundGen = GroundGenerator.Instance;
            _groundGen.GenerateGround(resolution, maxTerrainHeight, generalSettings, noiseSettings, noiseWithClamp,
                out map);
            _groundGen.GenerateWall(wallPrefab, resolution, maxTerrainHeight, generalSettings);

            Instantiate(waterGenerator, transform);
            var waterGen = WaterGenerator.Instance;
            waterGen.GenerateWater(resolution, maxTerrainHeight, waterLevel, generalSettings);

            return true;
        }
    }
}