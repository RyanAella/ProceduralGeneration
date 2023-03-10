// using System;
// using UnityEngine;
//
// namespace _Scripts
// {
//     /**
//      * This class generates a value per cell (x-, y-coordinate pair)
//      */
//     public static class ValueGenerator
//     {
//         public static int Evaluate(int x, int y, ValueGenerationSettings settings)
//         {
//             float threshold;
//             double noiseValue;
//
//             // Check if a random seed is wanted
//             if (settings.useRandomSeed)
//             {
//                 settings.SetSeed(Time.realtimeSinceStartupAsDouble.ToString());
//             }
//
//             // Get the coordinates
//             float seedOffset = settings.GetSeed().GetHashCode() / settings.seedScale;
//             var sampleX = (x + seedOffset) * settings.noiseScale;
//             var sampleY = (y + seedOffset) * settings.noiseScale;
//
//             // Interpolate between 0.0 and 1.0 by settings.percentage / 100
//             threshold = Mathf.Lerp(0.0f, 1.0f, (float)settings.percentage / 100);
//             noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
//
//
//             return noiseValue < threshold ? 1 : 0;
//         }
//     }
// }