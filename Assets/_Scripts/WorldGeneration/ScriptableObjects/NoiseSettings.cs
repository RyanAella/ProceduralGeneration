using UnityEngine;

namespace _Scripts.WorldGeneration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Noise")]
    public class NoiseSettings : ScriptableObject
    {
        // Gradient noise settings
        [Header("Gradient Noise")] [Range(0.0f, 0.1f)]
        public float noiseScale = 0.0768f;

        public int octaves = 8;
        public float persistence = 0.5f;
        public float lacunarity = 2.0f;
    }
}