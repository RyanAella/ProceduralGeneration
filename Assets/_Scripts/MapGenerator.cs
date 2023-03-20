using System;
using UnityEngine;

namespace _Scripts
{
    /**
     * This class controls the entire generation process.
     * All required parameters are collected in it and passed to the corresponding methods.
     */
    [Serializable]
    public class MapGenerator
    {
        // Resolution: default 16:9
        [SerializeField] private Vector2Int resolution = new(16, 16);

        // General
        // [Header("General")] [Range(0, 100)] public int fillPercentage = 45;

        // Seed
        [Header("Seed")] public bool useRandomSeed = false;
        public string seed = "Hello World!";
        [Range(1000.0f, 1000000.0f)] public float seedScale = 100000.0f;

        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 1.0f)] [SerializeField]
        private float noiseScale = 0.325f;

        [SerializeField] private int octaves = 3;
        [SerializeField] private float amplitude = 0.05f;
        [SerializeField] private float frequency = 2.0f;

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


        float[,] map;

        public float[,] GenerateMap(Mesh mesh, Gradient gradient)
        {
            map = new float[resolution.x, resolution.y];
            FillMap();

            return map;
        }

        void FillMap()
        {
            // float threshold;
            float noiseValue;
            int width = resolution.x;
            int height = resolution.y;

            // Check if a random seed is wanted
            if (useRandomSeed)
            {
                // seed = Time.realtimeSinceStartupAsDouble.ToString();
            }

            float seedOffset = seed.GetHashCode() / seedScale;

            float fr = frequency;
            float amp = amplitude;

            for (int l = 0; l < octaves; l++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Get the coordinates
                        float sampleX = ((float)x + seedOffset) /*/ width*/ * noiseScale ;
                        float sampleZ = ((float)y + seedOffset) /*/ height*/ * noiseScale ;

                        // if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        // {
                        //     map[x, y] = 1;
                        //     break;
                        // }

                        fr = Mathf.Pow(frequency, l);
                        amp = Mathf.Pow(amplitude, l);

                        // Interpolate between 0.0 and 1.0 by settings.percentage / 100
                        // threshold = Mathf.Lerp(0.0f, 1.0f, (float)fillPercentage / 100);
                        noiseValue = (Mathf.PerlinNoise(sampleX + fr, sampleZ + fr));

                        noiseValue = amp * noiseValue;

                        map[x, y] += noiseValue;
                    }
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
    }
}