using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using UnityEngine.Serialization;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;

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

        [Tooltip("A list of assets to generate.")] [SerializeField]
        private List<AssetSettings> assets;

        // private
        public WorldManager instance;
        private TimeManager _timer;
        private AssetManager _assetManager;
        private NoiseWithClamp _noiseWithClamp;

        private Vector2[] _corners;
        private float[,] _map;
        private Transform _parent;
        private Dictionary<AssetSettings, Transform> _parents;

        private bool _running;
        private bool _terrainGenerated;
        private bool _assetsGenerated;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
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

            if (assets.Count != 0 && _terrainGenerated)
            {
                _parents = new Dictionary<AssetSettings, Transform>();
                foreach (var assetSetting in assets)
                {
                    // _parent = Instantiate(assetSetting.parent, transform);
                    _parents.Add(assetSetting, Instantiate(assetSetting.parent, transform));
                    assetSetting.assets = new List<GameObject>();
                    _assetManager.InitialSpawnAssets(resolution, maxTerrainHeight, generalSettings, _map, assetSetting,
                        _parents[assetSetting]);
                }

                _assetsGenerated = true;
            }

            _running = true;
        }

        // Update is called once per frame
        private void Update()
        {
            // Debug.Log(_timer.GetCurrentDate().ToString("/"));

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

                if (assets.Count != 0 && _terrainGenerated)
                {
                    foreach (var parent in _parents)
                    {
                        Destroy(parent.Value.gameObject);
                    }
                    
                    _parents = new Dictionary<AssetSettings, Transform>();
                    foreach (var assetSetting in assets)
                    {
                        // Destroy(_parents[assetSetting]);
                        _parent = Instantiate(assetSetting.parent, transform);
                        _parents.Add(assetSetting, _parent);
                        assetSetting.assets = new List<GameObject>();
                        _assetManager.InitialSpawnAssets(resolution, maxTerrainHeight, generalSettings, _map,
                            assetSetting,
                            _parent);
                    }

                    _assetsGenerated = true;
                }
            }

            if (_terrainGenerated && _assetsGenerated)
            {
                foreach (var assetSetting in assets)
                {
                    CheckAssets(resolution, maxTerrainHeight, generalSettings, _map, assetSetting, _parents[assetSetting]);

                    if (assetSetting.assets.Count < assetSetting.maxNumber)
                    {
                        var count = assetSetting.maxNumber - assetSetting.assets.Count;
                        // _parent = _parents[assetSetting];
                        _assetManager.SpawnAssets(resolution, maxTerrainHeight, generalSettings, _map, assetSetting,
                            _parents[assetSetting], count);
                    }
                }
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

        private void CheckAssets(Vector2Int resolution, float maxTerrainHeight, GeneralSettings generalSettings,
            float[,] map, AssetSettings settings, Transform parent)
        {
            foreach (var assetSetting in assets)
            {
                // Debug.Log(assetSetting.assets.Count + " - " + assetSetting.maxNumber);
                //
                // if (assetSetting.assets.Count < assetSetting.maxNumber)
                // {
                //     var count = assetSetting.maxNumber - assetSetting.assets.Count;
                //     // _parent = _parents[assetSetting];
                //     _assetManager.SpawnAssets(resolution, maxTerrainHeight, generalSettings, map, assetSetting,
                //         parent, count);
                // }
            }
        }
    }
}