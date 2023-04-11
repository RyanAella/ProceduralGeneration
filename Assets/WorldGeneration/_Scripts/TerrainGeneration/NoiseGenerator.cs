using System;
using UnityEngine;

namespace _Scripts.Generator
{
    /// <summary>
    /// Generates a Noise value with or without Fractional Brownian Motion.
    /// </summary>
    [Serializable]
    public class NoiseGenerator
    {
        // Seed
        [Header("Seed")] public bool useRandomSeed = false;
        public string seed = "Hello World!";
        [Range(1000.0f, 1000000.0f)] public float seedScale = 100000.0f;

        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 1.0f)] [SerializeField]
        private float noiseScale = 0.325f;

        [SerializeField] private int octaves = 3;
        [SerializeField] private float persistence = 0.5f;
        [SerializeField] private float lacunarity = 2.0f;

        /// <summary>
        /// Takes two coordinates and generates a Noise value.
        /// </summary>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>A float value</returns>
        public float GenerateNoiseValue(float x, float y)
        {
            // Check if a random seed is wanted
            if (useRandomSeed)
            {
                seed = UnityEngine.Time.realtimeSinceStartupAsDouble.ToString();
            }

            float seedOffset = seed.GetHashCode() / seedScale;

            // Get the coordinates
            float sampleX = (x + seedOffset) * noiseScale;
            float sampleZ = (y + seedOffset) * noiseScale;

            return Mathf.PerlinNoise(sampleX, sampleZ);
        }

        /// <summary>
        /// Takes two coordinates and generates a Noise value with Fractional Brownian Motion.
        /// </summary>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>A float value</returns>
        public float GenerateNoiseValueWithFbm(float x, float y)
        {
            // Check if a random seed is wanted
            if (useRandomSeed)
            {
                seed = UnityEngine.Time.realtimeSinceStartupAsDouble.ToString();
            }

            float seedOffset = seed.GetHashCode() / seedScale;

            float value = 0.0f;
            float frequency = 1.0f;
            float amplitude = 1.0f;

            for (int l = 0; l < octaves; l++)
            {
                // Get the coordinates
                float sampleX = (x + seedOffset) * noiseScale;
                float sampleZ = (y + seedOffset) * noiseScale;

                // calculate noise value with modified amplitude and frequency
                value += amplitude * Mathf.PerlinNoise(sampleX * frequency, sampleZ * frequency);

                frequency *= lacunarity; // double the frequency each octave
                amplitude *= persistence; // half the amplitude
            }

            return value;
        }
    }
}