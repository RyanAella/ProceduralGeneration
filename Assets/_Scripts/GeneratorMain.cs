using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class GeneratorMain : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private Gradient gradient;
        private Mesh mesh;
        private MeshRenderer meshRenderer;

        private float[,] map;

        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float minTerrainHeight = 0.0f;
        [SerializeField] private float squareSize = 10.0f;
        
        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }
        
        void Start()
        {
        //     meshRenderer = GetComponent<MeshRenderer>();
        //     meshRenderer.enabled = false;
        //     
            mapGenerator = new MapGenerator();

            mesh = GetComponent<MeshFilter>().sharedMesh;
            
            map = mapGenerator.GenerateMap(mesh, gradient);
            
            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(mesh, gradient, map, maxTerrainHeight, squareSize);
            // meshRenderer.enabled = true;
        }

        // void OnValidate()
        // {
        // //     meshRenderer = GetComponent<MeshRenderer>();
        // //     meshRenderer.enabled = false;
        // //     
        //     mesh = GetComponent<MeshFilter>().sharedMesh;
        //
        //     mapGenerator.GenerateMap(mesh, gradient);
        //     // meshRenderer.enabled = true;
        // }
    }
}
