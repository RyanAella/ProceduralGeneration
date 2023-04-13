using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class PlantSettings : ScriptableObject
    {
        public InGameDate lifespan;
    }
}
