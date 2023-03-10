using UnityEngine;

namespace _Scripts
{
    /**
     * This class controls the entire generation process.
     * All required parameters are collected in it and passed to the corresponding methods.
     */
    public class MapGenerator : MonoBehaviour
    {
        // Resolution: default 16:9
        [SerializeField] private Vector2Int resolution = new(128, 72);

        // General
        [Header("General")] [Range(0, 100)] public int fillPercentage = 45;

        // Seed
        [Header("Seed")] public bool useRandomSeed = true;
        public string seed = "Hello World!";
        [Range(1000.0f, 1000000.0f)] public float seedScale = 100000.0f;

        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 1.0f)]
        public float noiseScale = 0.033f;
        
        // Height and Square size
        public float terrainHeight = 1.0f;
        public float squareSize = 1.0f;

        // Script access
        // private CellMapGenerator _cellMapGenerator;
        // [SerializeField] private TilemapGenerator tilemapGenerator;
        //
        // [Header("Settings for each Layer/Tilemap")]
        // // Settings for the base layer determining if a tile is in or outdoors
        // [Tooltip("What percentage is indoors.")]
        // [SerializeField] private ValueGenerationSettings baseLayerSettings;
        //
        // // Settings for determining if an indoor tile is massive rock or a cavity
        // [Tooltip("What percentage is massive rock.")]
        // [SerializeField] private ValueGenerationSettings mountainLayerSettings;
        //
        // // Settings for determining if an outdoor tile is meadows or woods
        // [Tooltip("What percentage is meadows.")]
        // [SerializeField] private ValueGenerationSettings outdoorBiomSettings;
        //
        // // Settings for determining if a meadows tile is water
        // [Tooltip("What percentage is water.")]
        // [SerializeField] private ValueGenerationSettings waterLayerSettings;
        //
        // [Header("Settings for Meadows/Woods Assets")]
        // // Settings for determining how many percent of meadows are trees, bushes and gras
        // [SerializeField] private AssetGenerationSettings meadowsAssetSettings;
        //
        // // Settings for determining how many percent of woods are trees, bushes and gras
        // [SerializeField] private AssetGenerationSettings woodsAssetSettings;
        //
        // // For the use of OnValidate()
        // private bool _scriptLoaded;
        //
        // // For Debugging
        // // private CellDebugger _debugger;


        double[,] map;

        void Start()
        {
            GenerateMap();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenerateMap();
            }
        }

        void GenerateMap()
        {
            map = new double[resolution.x, resolution.y];
            FillMap();

            MeshGenerator meshGen = GetComponent<MeshGenerator>();
            meshGen.GenerateMesh(map, terrainHeight, squareSize);
        }

        void FillMap()
        {
            float threshold;
            double noiseValue;
            int width = resolution.x;
            int height = resolution.y;

            // Check if a random seed is wanted
            if (useRandomSeed)
            {
                seed = Time.realtimeSinceStartupAsDouble.ToString();
            }

            float seedOffset = seed.GetHashCode() / seedScale;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Get the coordinates
                    float sampleX = (x + seedOffset) * noiseScale;
                    float sampleZ = (y + seedOffset) * noiseScale;

                    // if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    // {
                    //     map[x, y] = 1;
                    //     break;
                    // }

                    // Interpolate between 0.0 and 1.0 by settings.percentage / 100
                    threshold = Mathf.Lerp(0.0f, 1.0f, (float)fillPercentage / 100);
                    noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);

                    map[x, y] = noiseValue; //(noiseValue < threshold) ? 1 : 0;
                    Debug.Log("noiseValue: " + noiseValue);
                }
            }
        }

        // int GetSurroundingWallCount(int gridX, int gridY)
        // {
        //     int wallCount = 0;
        //     for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        //     {
        //         for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
        //         {
        //             if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
        //             {
        //                 if (neighbourX != gridX || neighbourY != gridY)
        //                 {
        //                     wallCount += map[neighbourX, neighbourY];
        //                 }
        //             }
        //             else
        //             {
        //                 wallCount++;
        //             }
        //         }
        //     }
        //
        //     return wallCount;
        // }


        // void OnDrawGizmos()
        // {
        //     if (map != null)
        //     {
        //         for (int x = 0; x < resolution.x; x++)
        //         {
        //             for (int y = 0; y < resolution.y; y++)
        //             {
        //                 Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
        //                 Vector3 pos = new Vector3(-resolution.x / 2 + x + .5f, 0, -resolution.y / 2 + y + .5f);
        //                 Gizmos.DrawCube(pos, Vector3.one);
        //             }
        //         }
        //     }
        // }
    }
}