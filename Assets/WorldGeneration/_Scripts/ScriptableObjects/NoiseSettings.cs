using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Noise")]
    public class NoiseSettings : ScriptableObject
    {
        // Seed
        [Header("Seed")] public bool useRandomSeed;
        public string seed = "Hello World!";
        // [Range(10000.0f, 1000000.0f)] public float seedScale = 100000.0f;

        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 0.1f)]
        public float noiseScale = 0.0260522f;

        public int octaves = 3;
        public float persistence = 0.5f;
        public float lacunarity = 2.0f;
    }
}