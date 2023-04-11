using _Scripts.Helper;
using _Scripts.ScriptableObjects;
using _Scripts.Time;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// This class controls the generation of the ground and water maps and meshes.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class GroundGenerator : MonoBehaviour
    {
        // [SerializeField]
        [SerializeField] private GeneralSettings generalSettings;
        [SerializeField] private NoiseGenerator noiseGenerator;
        [SerializeField] private Gradient gradient;

        // public
        public GameObject DemoCarrot;
        // public GameObject DemoBurrow;
        
        // private
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        
        private Spawner _spawner;
        private TimeManager _timer;
        private ValueClamp _clamp;

        private float[,] _map;
        private Vector2[] _boundaries;

        // For the use of OnValidate()
        private bool _scriptLoaded;

        // Only for Debugging
        private bool _running;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;

            _timer = TimeManager.Instance;
            _clamp = new ValueClamp();
            _spawner = new Spawner();
            noiseGenerator = new NoiseGenerator();

            _mesh = new Mesh()
            {
                name = "Ground Mesh"
            };

            gameObject.tag = "Ground";
            gameObject.layer = LayerMask.NameToLayer("Ground");

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            _map = new float[generalSettings.resolution.x, generalSettings.resolution.y];

            for (int x = 0; x < generalSettings.resolution.x; x++)
            {
                for (int y = 0; y < generalSettings.resolution.y; y++)
                {
                    float sampleX = x - generalSettings.resolution.x / 2;
                    float sampleY = y - generalSettings.resolution.y / 2;

                    // _map[x, y] = _noiseGenerator.GenerateNoiseValue(sampleX, sampleY);
                    _map[x, y] = noiseGenerator.GenerateNoiseValueWithFbm(sampleX, sampleY);
                    
                    _clamp.Compare(_map[x, y]);
                }
            }

            // Get each point back into bounds
            for (int x = 0; x < generalSettings.resolution.x; x++)
            {
                for (int y = 0; y < generalSettings.resolution.y; y++)
                {
                    _map[x, y] = _clamp.ClampValue(_map[x, y]);
                }
            }

            MeshGenerator.GenerateMesh(_mesh, _map, generalSettings);
            ColorGenerator.AssignColor(gradient, _mesh, generalSettings.maxTerrainHeight);

            // _spawner.SpawnCarrot(DemoCarrot, GeneratorFunctions.GetSurfacePoint(2.3f, 4.7f, generalSettings, noiseGenerator, _clamp));
            // _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(5.4f, 4.7f, generalSettings, noiseGenerator, _clamp));
            _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(0.0f, 0.0f, generalSettings, noiseGenerator, _clamp));
            _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(8.0f, 8.0f, generalSettings, noiseGenerator, _clamp));
            _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(15.0f, 15.0f, generalSettings, noiseGenerator, _clamp));
            // _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(14.6f, 8.7f, generalSettings, noiseGenerator, _clamp));
            // _spawner.SpawnBurrow(DemoCarrot, GeneratorFunctions.GetSurfacePoint(3.9f, 12.7f, generalSettings, noiseGenerator, _clamp));

            _scriptLoaded = true;
            _running = true;
        }

        // private void OnValidate()
        // {
        //     if (!_scriptLoaded) return;
        //
        //     // später WaterGenerator und sein Object instantiaten
        //
        //     _mesh = new Mesh()
        //     {
        //         name = "Ground Mesh"
        //     };
        //
        //     gameObject.tag = "Ground";
        //     gameObject.layer = LayerMask.NameToLayer("Ground");
        //
        //     GetComponent<MeshFilter>().sharedMesh = _mesh;
        //
        //     // groundGenerator = new GroundGenerator();
        //     // _map = new float[generalSettings.resolution.x, generalSettings.resolution.y];
        //     // _map = groundGenerator.GenerateMap(_map);
        //
        //     MeshGenerator.GenerateMesh(_mesh, _map, generalSettings.maxTerrainHeight, generalSettings.squareSize);
        //     ColorGenerator.AssignColor(gradient, _mesh, generalSettings.maxTerrainHeight);
        // }

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

            if (Input.GetKeyDown(KeyCode.Alpha1) && _running)
            {
                _timer.SetTimeScale(1.0f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && _running)
            {
                _timer.SetTimeScale(2.0f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && _running)
            {
                _timer.SetTimeScale(3.0f);
            }
        }
    }
}