using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/General")]
    public class GeneralSettings : ScriptableObject
    {
        [Tooltip("The size per square.")]
        public float squareSize = 80.0f;

        [Tooltip("The maximum height of the surrounding walls.")]
        public float maxBorderHeight = 50.0f;
    }
}