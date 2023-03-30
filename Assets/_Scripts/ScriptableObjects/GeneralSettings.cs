using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/General")]
    public class GeneralSettings : ScriptableObject
    {
        public Vector2Int resolution = new(16, 16);
        public float maxTerrainHeight = 10.0f;
        public float squareSize = 10.0f;
    }
}
