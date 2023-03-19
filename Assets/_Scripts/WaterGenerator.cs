using System;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
    public class WaterGenerator : MonoBehaviour
    {
        // Resolution: default 16:9
        [SerializeField] private Vector2Int resolution = new(16, 16);
        
        private float[,] map;
        private Mesh mesh;
        private MeshRenderer meshRenderer;
        
        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float squareSize = 10.0f;
        
        // For the use of OnValidate()
        private bool _scriptLoaded;

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }
        
        // Generieren des WaterMeshes
        void Start()
        {
            // Debug.Log(Mathf.Lerp(-1.0f, 1.0f, 0.5f));
            
            mesh = new Mesh()
            {
                name = "Water Mesh"
            };
            
            gameObject.tag = "Water";
            gameObject.layer = LayerMask.NameToLayer("Water");
            
            map = new float[resolution.x, resolution.y];
            
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
            
            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(mesh, map, maxTerrainHeight, squareSize);
            
            GetComponent<MeshFilter>().sharedMesh = mesh;
            
            _scriptLoaded = true;
        }
        
        // Generieren des WaterMeshes
        void OnValidate()
        {
            if (!_scriptLoaded) return;
            
            // Debug.Log(Mathf.Lerp(-1.0f, 1.0f, 0.5f));
            
            mesh = new Mesh()
            {
                name = "Water Mesh"
            };

            gameObject.tag = "Water";
            gameObject.layer = LayerMask.NameToLayer("Water");
            
            map = new float[resolution.x, resolution.y];
            
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
            
            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(mesh, map, maxTerrainHeight, squareSize);
            
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        // Veränderung der Wellen
        void Update()
        {
        }
    }
}