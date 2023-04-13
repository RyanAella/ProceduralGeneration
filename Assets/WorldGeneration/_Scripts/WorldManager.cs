using Time;
using UnityEngine;
using WorldGeneration._Scripts.Helper;
using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.Spawning;
using WorldGeneration._Scripts.TerrainGeneration;

namespace WorldGeneration._Scripts
{
    public class WorldManager : MonoBehaviour
    {
        // [SerializeField]
        [Header("General Settings")]
        [SerializeField] private Vector2Int resolution = new(128, 128);
        [SerializeField] private float maxTerrainHeight = 180.0f;
        [Range(0.0f, 1.0f)][SerializeField] private float waterLevel = 0.3f;
        [SerializeField] private GeneralSettings generalSettings;
        [SerializeField] private NoiseSettings noiseSettings;
        
        [Header("Objects")]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject demoCarrot;
        [SerializeField] private int maxNumberOfCarrots = 20;

        // private
        private TerrainManager _terrainManager;
        private AssetManager _assetManager;
        private TimeManager _timer;

        private NoiseWithClamp _noiseWithClamp;

        private bool _running;
        private Vector2[] _corners;

        // Start is called before the first frame update
        private void Start()
        {
            _terrainManager = TerrainManager.GetInstance();
            _assetManager = AssetManager.GetInstance();
            _timer = TimeManager.Instance;

            _noiseWithClamp.NoiseGenerator = new NoiseGenerator();
            _noiseWithClamp.ValueClamp = new ValueClamp();

            _terrainManager.GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings, noiseSettings, wallPrefab,
                _noiseWithClamp);

            _corners = GeneratorFunctions.GetCornerPoints(generalSettings, noiseSettings, _noiseWithClamp);

            _assetManager.SpawnAssets(resolution, maxTerrainHeight, waterLevel, generalSettings, noiseSettings, _noiseWithClamp,
                demoCarrot, _corners, maxNumberOfCarrots);

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
                _terrainManager.GenerateTerrain(resolution, maxTerrainHeight, waterLevel, generalSettings, noiseSettings, wallPrefab,
                    _noiseWithClamp);

                _corners = GeneratorFunctions.GetCornerPoints(generalSettings, noiseSettings, _noiseWithClamp);

                _assetManager.SpawnAssets(resolution, maxTerrainHeight, waterLevel, generalSettings, noiseSettings, _noiseWithClamp,
                    demoCarrot, _corners, maxNumberOfCarrots);
            }
        }
    }
}