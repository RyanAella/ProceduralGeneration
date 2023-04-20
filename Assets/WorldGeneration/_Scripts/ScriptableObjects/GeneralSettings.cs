using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/General")]
    public class GeneralSettings : ScriptableObject
    {
        [Header("Generation Settings")]
        // public Vector2Int resolution = new(16, 16);

        // public float maxTerrainHeight = 10.0f;
        [Tooltip("The size per square.")]
        public float squareSize = 80.0f;

        [Tooltip("The maximum height of the surrounding walls.")]
        public float maxBorderHeight = 50.0f;

        [Header("Game Settings")] [Tooltip("The number of rabbit burrows at the very beginning.")]
        public int initialRabbitBurrows = 1;

        [Tooltip("The number of fox burrows at the very beginning.")]
        public int initialFoxBurrows = 1;
    }
}