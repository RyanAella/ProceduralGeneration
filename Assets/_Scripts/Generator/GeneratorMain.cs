using _Scripts.Time;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// This class controls the generation of the ground and water maps and meshes.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class GeneratorMain : MonoBehaviour
    {
        [SerializeField] private Vector2Int resolution = new(16, 16);

        [SerializeField] private GroundGenerator groundGenerator;
        [SerializeField] private Gradient gradient;

        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        private float[,] _map;

        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float squareSize = 10.0f;

        // For the use of OnValidate()
        private bool _scriptLoaded;

        private TimeManager _timer;
        
        // Only for Debugging
        private bool _running;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;
            
            // ingame timer reference
            _timer = TimeManager.Instance;

            // später WaterGenerator und sein Object instantiaten

            _mesh = new Mesh()
            {
                name = "Ground Mesh"
            };
            
            gameObject.tag = "Ground";
            gameObject.layer = LayerMask.NameToLayer("Ground");

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            groundGenerator = new GroundGenerator();
            _map = new float[resolution.x, resolution.y];
            _map = groundGenerator.GenerateMap(_map);
            
            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, squareSize);
            ColorGenerator.AssignColor(gradient, _mesh, maxTerrainHeight);

            _scriptLoaded = true;
            _running = true;
        }

        void OnValidate()
        {
            if (!_scriptLoaded) return;

            // später WaterGenerator und sein Object instantiaten

            _mesh = new Mesh()
            {
                name = "Ground Mesh"
            };
            
            gameObject.tag = "Ground";
            gameObject.layer = LayerMask.NameToLayer("Ground");

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            groundGenerator = new GroundGenerator();
            _map = new float[resolution.x, resolution.y];
            _map = groundGenerator.GenerateMap(_map);
            
            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, squareSize);
            ColorGenerator.AssignColor(gradient, _mesh, maxTerrainHeight);

        }

        private void Update()
        {
            // Debug.Log(_timer.GetCurrentDate().ToString("/"));

            // Only for Debugging
            if (Input.GetKeyDown(KeyCode.P) && _running)
            {
                _timer.Stop();
                _running = false;
            } else if (Input.GetKeyDown(KeyCode.P) && !_running)
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