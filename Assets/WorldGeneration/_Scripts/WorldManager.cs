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
        [SerializeField] private Vector2Int resolution = new(128, 128);
        [SerializeField] private float maxTerrainHeight = 180.0f;
        [SerializeField] private GeneralSettings generalSettings;
        [SerializeField] private NoiseSettings noiseSettings;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject demoCarrot;
        [SerializeField] private AssetManager assetManager;
        private ValueClamp _clamp;
        private NoiseGenerator _noiseGenerator;

        private bool _running;
        private Vector2[] _corners;

        // private
        private TerrainManager _terrainManager;
        private TimeManager _timer;

        // Start is called before the first frame update
        private void Start()
        {
            assetManager = AssetManager.GetInstance();

            _timer = TimeManager.Instance;

            _terrainManager = TerrainManager.GetInstance();

            _noiseGenerator = new NoiseGenerator();
            _clamp = new ValueClamp();

            _terrainManager.GenerateTerrain(resolution, maxTerrainHeight, generalSettings, noiseSettings, wallPrefab,
                _clamp);

            _corners = GeneratorFunctions.GetCornerPoints(generalSettings, noiseSettings, _noiseGenerator,
                _clamp);

            assetManager.SpawnAssets(resolution, maxTerrainHeight, generalSettings, noiseSettings, _noiseGenerator,
                demoCarrot, _corners, _clamp);


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
                _terrainManager.GenerateTerrain(resolution, maxTerrainHeight, generalSettings, noiseSettings,
                    wallPrefab, _clamp);
                assetManager.SpawnAssets(resolution, maxTerrainHeight, generalSettings, noiseSettings, _noiseGenerator,
                    demoCarrot, _corners, _clamp);
            }
        }
    }
}