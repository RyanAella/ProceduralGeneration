using System;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// Class <c>GroundGenerator</c> (Singleton) generates the ground map.
    /// </summary>
    [Serializable]
    public class GroundGenerator
    {
        // Seed
        [Header("Seed")] public bool useRandomSeed = false;
        public string seed = "Hello World!";
        [Range(1000.0f, 1000000.0f)] public float seedScale = 100000.0f;

        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 1.0f)] [SerializeField] private float noiseScale = 0.325f;
        [SerializeField] private int octaves = 3;
        [SerializeField] private float persistence = 0.5f;
        [SerializeField] private float lacunarity = 2.0f;

        /** 
         * <param name="map"> The map which needs to contain the heightmap.</param>
         * <returns> The final heightmap</returns>
         */
        public float[,] GenerateMap(float[,] map)
        {
            // setting min and max value for comparison
            float maxValue = float.MinValue;    // initialize maxValue to the smallest possible float value, 
            float minValue = float.MaxValue;    // initialize minValue to the biggest possible float value, -> both will be changed
            
            float width = map.GetLength(0);
            float height = map.GetLength(1);
            
            // Check if a random seed is wanted
            if (useRandomSeed)
            {
                seed = UnityEngine.Time.realtimeSinceStartupAsDouble.ToString();
            }

            float seedOffset = seed.GetHashCode() / seedScale;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = 0.0f;
                    float frequency = 1.0f;
                    float amplitude = 1.0f;
                    
                    for (int l = 0; l < octaves; l++)
                    {
                        // Get the coordinates
                        float sampleX = ((x - width / 2) + seedOffset) * noiseScale ;
                        float sampleZ = ((y - height / 2) + seedOffset) * noiseScale ;

                        // calculate noise value with modified amplitude and frequency
                        value += amplitude * Mathf.PerlinNoise(sampleX * frequency, sampleZ * frequency);
                        
                        frequency *= lacunarity;  // double the frequency each octave
                        amplitude *= persistence;  // half the amplitude
                    }

                    // Get the new min and max values
                    if (value > maxValue)
                    {
                        maxValue = value;
                    } else if (value < minValue)
                    {
                        minValue = value;
                    }

                    // add value to point xy
                    map[x, y] = value;
                }
            }
            
            // Get each point back into bounds
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = Mathf.InverseLerp(minValue, maxValue, map[x, y]);
                }
            }
            
            return map;
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