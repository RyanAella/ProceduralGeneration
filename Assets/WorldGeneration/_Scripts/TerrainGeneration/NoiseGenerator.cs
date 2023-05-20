using WorldGeneration._Scripts.ScriptableObjects;
using WorldGeneration._Scripts.TerrainGeneration.NoiseTest;

namespace WorldGeneration._Scripts.TerrainGeneration
{
    /// <summary>
    ///     Generates a Noise value with or without Fractional Brownian Motion.
    /// </summary>
    public class NoiseGenerator
    {
        private readonly OpenSimplexNoise _simplexNoise;

        public NoiseGenerator(string seed)
        {
            _simplexNoise = new OpenSimplexNoise(seed.GetHashCode());
        }

        /// <summary>
        ///     Takes two coordinates and generates a Noise value.
        /// </summary>
        /// <param name="noiseSettings"></param>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>A float value</returns>
        private float GenerateNoiseValue(NoiseSettings noiseSettings, float x, float y)
        {
            // var seedOffset = noiseSettings.seed.GetHashCode() / noiseSettings.seedScale;

            // Get the coordinates
            var sampleX = x /*+ seedOffset*/ * noiseSettings.noiseScale;
            var sampleZ = y /*+ seedOffset*/ * noiseSettings.noiseScale;

            // return Mathf.PerlinNoise(sampleX, sampleZ);
            return (float)_simplexNoise.Evaluate(sampleX, sampleZ);
        }

        /// <summary>
        ///     Takes two coordinates and generates a Noise value with Fractional Brownian Motion.
        /// </summary>
        /// <param name="noiseSettings"></param>
        /// <param name="x">xCoordinate</param>
        /// <param name="y">yCoordinate</param>
        /// <returns>A float value</returns>
        public float GenerateNoiseValueWithFbm(NoiseSettings noiseSettings, float x, float y)
        {
            // var seedOffset = noiseSettings.seed.GetHashCode() / noiseSettings.seedScale;

            var value = 0.0f;
            var frequency = 1.0f;
            var amplitude = 1.0f;

            for (var l = 0; l < noiseSettings.octaves; l++)
            {
                // calculate noise value with modified amplitude and frequency
                // value += amplitude * Mathf.PerlinNoise(sampleX * frequency, sampleZ * frequency);
                value += amplitude * GenerateNoiseValue(noiseSettings, x * frequency, y * frequency);

                frequency *= noiseSettings.lacunarity; // double the frequency each octave
                amplitude *= noiseSettings.persistence; // half the amplitude
            }

            return value;
        }
    }
}