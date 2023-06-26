/*
* Copyright (c) mmq
*/

using _Scripts.WorldGeneration.ScriptableObjects;
using _Scripts.WorldGeneration.TerrainGeneration.NoiseTest;

namespace _Scripts.WorldGeneration.TerrainGeneration
{
    /// <summary>
    /// Generates a Noise value with or without Fractional Brownian Motion.
    /// </summary>
    public class NoiseGenerator
    {
        #region Variables
        
        private readonly OpenSimplexNoise _simplexNoise;
        
        #endregion

        #region Unity Methods
        
        public NoiseGenerator(string seed)
        {
            _simplexNoise = new OpenSimplexNoise(seed.GetHashCode());
        }

        /// <summary>
        /// Takes two coordinates and generates a Noise value.
        /// </summary>
        /// <param name="noiseSettings">The noise setting needed for the mesh generation</param>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>The height at the given coordinate as a float value</returns>
        private float GenerateNoiseValue(NoiseSettings noiseSettings, float x, float y)
        {
            // Get the coordinates
            var sampleX = x * noiseSettings.noiseScale;
            var sampleZ = y * noiseSettings.noiseScale;

            return (float)_simplexNoise.Evaluate(sampleX, sampleZ);
        }

        /// <summary>
        /// Takes two coordinates and generates a Noise value with Fractional Brownian Motion.
        /// </summary>
        /// <param name="noiseSettings">The noise setting needed for the mesh generation</param>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>The height at the given coordinate as a float value</returns>
        public float GenerateNoiseValueWithFbm(NoiseSettings noiseSettings, float x, float y)
        {
            var value = 0.0f;
            var frequency = 1.0f;
            var amplitude = 1.0f;

            for (var l = 0; l < noiseSettings.octaves; l++)
            {
                // calculate noise value with modified amplitude and frequency
                value += amplitude * GenerateNoiseValue(noiseSettings, x * frequency, y * frequency);

                frequency *= noiseSettings.lacunarity; // double the frequency each octave
                amplitude *= noiseSettings.persistence; // half the amplitude
            }

            return value;
        }
        
        #endregion
    }
}