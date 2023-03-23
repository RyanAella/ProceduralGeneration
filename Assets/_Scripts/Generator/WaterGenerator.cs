using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// This class generates the water map.
    /// </summary>
    public class WaterGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int resolution = new(16, 16);
        
        private float[,] _map;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        
        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float squareSize = 10.0f;
        
        // For the use of OnValidate()
        private bool _scriptLoaded;

        public static WaterGenerator Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;
            
            _mesh = new Mesh()
            {
                name = "Water Mesh"
            };
            
            gameObject.tag = "Water";
            gameObject.layer = LayerMask.NameToLayer("Water");
            
            _map = new float[resolution.x, resolution.y];
            
            // // System.Random prng = new System.Random();
            // float prng = Random.Range(0.0f, 1.0f);
            //
            // int width = resolution.x;
            // int height = resolution.y;
            //
            // for (int x = 0; x < width; x++)
            // {
            //     for (int y = 0; y < height; y++)
            //     {
            //         map[x, y] = 0.2f;//Mathf.Lerp(0.0f, 1.0f, (float) prng);
            //     }
            // }
            
            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, squareSize);
            
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            
            _scriptLoaded = true;
        }

        private void OnValidate()
        {
            if (!_scriptLoaded) return;
            
            // Debug.Log(Mathf.Lerp(-1.0f, 1.0f, 0.5f));
            
            _mesh = new Mesh()
            {
                name = "Water Mesh"
            };

            gameObject.tag = "Water";
            gameObject.layer = LayerMask.NameToLayer("Water");
            
            _map = new float[resolution.x, resolution.y];
            
            // // System.Random prng = new System.Random();
            // float prng = Random.Range(0.0f, 1.0f);
            //
            // int width = resolution.x;
            // int height = resolution.y;
            //
            // for (int x = 0; x < width; x++)
            // {
            //     for (int y = 0; y < height; y++)
            //     {
            //         map[x, y] = 0.2f;//Mathf.Lerp(0.0f, 1.0f, (float) prng);
            //     }
            // }
            
            MeshGenerator.GenerateMesh(_mesh, _map, maxTerrainHeight, squareSize);
            
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }
    }
}