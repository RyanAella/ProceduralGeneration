using System;
using _Scripts.ScriptableObjects;
using _Scripts.Time;
using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class GeneratorMain : MonoBehaviour
    {
        // Resolution: default 16:9
        [SerializeField] private Vector2Int resolution = new(16, 16);

        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private Gradient gradient;

        private Mesh mesh;
        private MeshRenderer meshRenderer;

        private float[,] map;

        // Height and Square size
        [SerializeField] private float maxTerrainHeight = 10.0f;
        [SerializeField] private float squareSize = 10.0f;

        // For the use of OnValidate()
        private bool _scriptLoaded;

        private TimeManager _timer;


        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }

        void Start()
        {
            // ingame timer reference
            _timer = TimeManager.Instance;

            // später WaterGenerator und sein Object instantiaten

            mesh = new Mesh()
            {
                name = "Ground Mesh"
            };

            GetComponent<MeshFilter>().sharedMesh = mesh;

            mapGenerator = new MapGenerator();
            map = new float[resolution.x, resolution.y];
            // map = mapGenerator.GenerateMap(resolution, map);
            map = mapGenerator.GenerateMap2(map);

            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(mesh, map, maxTerrainHeight, squareSize);

            ColorGenerator colorGenerator = new ColorGenerator();
            colorGenerator.AssignColor(gradient, mesh, maxTerrainHeight);

            _scriptLoaded = true;
        }

        void OnValidate()
        {
            if (!_scriptLoaded) return;

            // später WaterGenerator und sein Object instantiaten

            mesh = new Mesh()
            {
                name = "Ground Mesh"
            };

            GetComponent<MeshFilter>().sharedMesh = mesh;

            mapGenerator = new MapGenerator();
            map = new float[resolution.x, resolution.y];
            // map = mapGenerator.GenerateMap(resolution, map);
            map = mapGenerator.GenerateMap2(map);

            MeshGenerator meshGen = new MeshGenerator();
            meshGen.GenerateMesh(mesh, map, maxTerrainHeight, squareSize);

            ColorGenerator colorGenerator = new ColorGenerator();
            colorGenerator.AssignColor(gradient, mesh, maxTerrainHeight);
        }


        // private void Update()
        // {
        //     Debug.Log(
        //         $"Year {timer.Year:00}, Month {timer.Month:00}, Week {timer.Week:00}");
        // }
    }
}