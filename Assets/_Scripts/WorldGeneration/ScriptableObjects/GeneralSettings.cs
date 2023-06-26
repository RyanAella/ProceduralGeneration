using UnityEngine;

namespace _Scripts.WorldGeneration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/General")]
    public class GeneralSettings : ScriptableObject
    {
        public string seed = "Labor Games 2023";
        public bool useRandomSeed = true;

        [Range(25, 200)] public float maxTerrainHeight = 180.0f;

        public Vector2Int resolution = new(128, 128);

        [Tooltip("The size per square.")] public float squareSize = 80.0f;

        [Tooltip("The maximum height of the surrounding walls.")]
        public float maxBorderHeight = 50.0f;

        [Tooltip("Only for menu")] public WatchMode watchMode = WatchMode.Default; // 

        public bool isStartingFromMenu;
    }

    public enum WatchMode
    {
        Default,
        FlyCam,
        FirstPerson
    }
}