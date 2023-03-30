using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// This class generates the water map.
    /// </summary>
    public class WaterGenerator : MonoBehaviour
    {
        // [SerializeField]
        [SerializeField] private GeneralSettings generalSettings;

        // private
        private static WaterGenerator _instance;
        
        private float[,] _map;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        // For the use of OnValidate()
        private bool _scriptLoaded;

        private void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }
            else if (_instance != this)
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

            _map = new float[generalSettings.resolution.x, generalSettings.resolution.y];

            // // System.Random prng = new System.Random();
            // float prng = Random.Range(0.0f, 1.0f);
            //
            // int width = generalSettings.resolution.x;
            // int height = generalSettings.resolution.y;
            //
            // for (int x = 0; x < width; x++)
            // {
            //     for (int y = 0; y < height; y++)
            //     {
            //         map[x, y] = 0.2f;//Mathf.Lerp(0.0f, 1.0f, (float) prng);
            //     }
            // }

            MeshGenerator.GenerateMesh(_mesh, _map, generalSettings);

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            _scriptLoaded = true;
        }

        // private void OnValidate()
        // {
        //     if (!_scriptLoaded) return;
        //     
        //     // Debug.Log(Mathf.Lerp(-1.0f, 1.0f, 0.5f));
        //     
        //     _mesh = new Mesh()
        //     {
        //         name = "Water Mesh"
        //     };
        //
        //     gameObject.tag = "Water";
        //     gameObject.layer = LayerMask.NameToLayer("Water");
        //     
        //     _map = new float[generalSettings.resolution.x, generalSettings.resolution.y];
        //     
        //     // // System.Random prng = new System.Random();
        //     // float prng = Random.Range(0.0f, 1.0f);
        //     //
        //     // int width = generalSettings.resolution.x;
        //     // int height = generalSettings.resolution.y;
        //     //
        //     // for (int x = 0; x < width; x++)
        //     // {
        //     //     for (int y = 0; y < height; y++)
        //     //     {
        //     //         map[x, y] = 0.2f;//Mathf.Lerp(0.0f, 1.0f, (float) prng);
        //     //     }
        //     // }
        //     
        //     MeshGenerator.GenerateMesh(_mesh, _map, generalSettings.maxTerrainHeight, squareSize);
        //     
        //     GetComponent<MeshFilter>().sharedMesh = _mesh;
        // }
    }
}