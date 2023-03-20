using System;
using _Scripts.Time;
using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class GeneratorMain : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private Gradient gradient;
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;

        private float[,] _map;

        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float minTerrainHeight = 0.0f;
        [SerializeField] private float squareSize = 10.0f;

        private TimeManager _timer;

        void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = true;
        }

        void Start()
        {
            _timer = TimeManager.Instance;

            //     meshRenderer = GetComponent<MeshRenderer>();
            //     meshRenderer.enabled = false;
            //     
            mapGenerator = new MapGenerator();

            _mesh = GetComponent<MeshFilter>().sharedMesh;

            _map = mapGenerator.GenerateMap(_mesh, gradient);

            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(_mesh, gradient, _map, maxTerrainHeight, squareSize);
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

        // private void Update()
        // {
        //     Debug.Log(
        //         $"Year {timer.Year:00}, Month {timer.Month:00}, Week {timer.Week:00}");
        // }
    }
}